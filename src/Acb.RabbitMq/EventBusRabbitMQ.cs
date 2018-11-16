using Acb.Core.EventBus;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Acb.RabbitMq
{
    public class EventBusRabbitMq : AbstractEventBus, IDisposable
    {
        private readonly string _brokerName;

        private readonly IRabbitMqConnection _connection;
        private readonly ILogger _logger;

        private IModel _consumerChannel;
        private string _queueName;
        private const string DelayTimesKey = "delay_times";

        public EventBusRabbitMq(IRabbitMqConnection connection, ISubscriptionManager subsManager) : base(subsManager)
        {
            _connection =
                connection ?? throw new ArgumentNullException(nameof(connection));
            _brokerName = connection.Broker;
            _logger = LogManager.Logger<EventBusRabbitMq>();
            SubscriptionManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        /// <summary> 获取订阅的队列信息 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static SubscriptionAttribute GetSubscription(Type type)
        {
            var attr = type.GetCustomAttribute<SubscriptionAttribute>() ?? new SubscriptionAttribute(type.FullName);
            if (string.IsNullOrWhiteSpace(attr.Queue))
                attr.Queue = type.FullName;
            return attr;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(_queueName, _brokerName, eventName);

                if (!SubscriptionManager.IsEmpty)
                    return;
                _queueName = string.Empty;
                _consumerChannel?.Close();
            }
        }

        public override async Task Publish(string key, object @event, long delay = 0, IDictionary<string, object> headers = null)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { _logger.Warn(ex.ToString()); });
            await policy.ExecuteAsync(async () =>
            {
                using (var channel = _connection.CreateModel())
                {
                    var message = @event.GetType().IsSimpleType()
                        ? @event.ToString()
                        : JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);
                    var prop = channel.CreateBasicProperties();
                    prop.DeliveryMode = 2;
                    if (headers != null)
                    {
                        if (prop.Headers == null)
                            prop.Headers = new Dictionary<string, object>();
                        foreach (var header in headers)
                        {
                            prop.Headers.AddOrUpdate(header.Key, header.Value);
                        }
                    }
                    //声明Exchange
                    channel.ExchangeDeclare(_brokerName, ExchangeType.Topic, true);
                    if (delay > 0)
                    {
                        channel.DelayPublish(_brokerName, key, body, delay, prop);
                    }
                    else
                    {
                        channel.BasicPublish(_brokerName, key, prop, body);
                    }
                }

                await Task.CompletedTask;
            });
        }

        private static TimeSpan DelayRule(int times)
        {
            if (times <= 3)
                return TimeSpan.FromSeconds(Math.Pow(10, times));
            return TimeSpan.FromHours(times == 4 ? 12 : 24);
        }

        /// <summary> 定义并绑定队列 </summary>
        /// <param name="queue"></param>
        /// <param name="key"></param>
        private void DeclareAndBindQueue(string queue, string key)
        {
            _consumerChannel.ExchangeDeclare(_brokerName, ExchangeType.Topic, true);
            _consumerChannel.DeclareWithDlx(queue, _brokerName, key);
        }

        /// <summary> 接收消息 </summary>
        /// <param name="queue">队列</param>
        /// <param name="ea"></param>
        /// <returns></returns>
        private async Task ReceiveMessage(string queue, BasicDeliverEventArgs ea)
        {
            //var name = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body);
            try
            {
                await ProcessEvent(queue, message);
                _consumerChannel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                //非业务异常,可重新入队
                if (ex.GetBaseException() is BusiException busi)
                {
                    //拒收，不重新入列
                    _consumerChannel.BasicNack(ea.DeliveryTag, false, false);
                    _logger.Warn($"{queue},busi:{busi.Message}");
                    return;
                }

                var times = 0;
                if (ea.BasicProperties.Headers != null &&
                    ea.BasicProperties.Headers.TryGetValue(DelayTimesKey, out var t))
                {
                    times = t.CastTo(0);
                }

                if (times > 5)
                {
                    //拒收，不重新入列
                    _consumerChannel.BasicNack(ea.DeliveryTag, false, false);
                    _logger.Warn($"{queue},retry times > 5");
                    return;
                }

                //延时入列
                times++;
                _consumerChannel.BasicAck(ea.DeliveryTag, false);

                if (!_connection.IsConnected)
                    _connection.TryConnect();
                using (var channel = _connection.CreateModel())
                {
                    var prop = _consumerChannel.CreateBasicProperties();
                    prop.DeliveryMode = 2;
                    prop.Headers = prop.Headers ?? new Dictionary<string, object>();
                    prop.Headers.Add(DelayTimesKey, times);
                    var delay = (long)DelayRule(times).TotalMilliseconds;
                    //绑定队列路由
                    channel.QueueBind(queue, _brokerName, queue);

                    channel.DelayPublish(_brokerName, queue, ea.Body, delay, prop);
                }

                //_logger.Error(ex.Message, ex);
                _logger.Warn(ex.GetBaseException().Message);

            }
        }

        public override Task Subscribe<T, TH>(Func<TH> handler)
        {
            _consumerChannel = _consumerChannel ?? CreateConsumerChannel();

            var subscription = GetSubscription(typeof(TH));
            var queue = subscription.Queue;
            var dataType = typeof(T);
            string key;
            if (typeof(DEvent).IsAssignableFrom(dataType))
            {
                key = !string.IsNullOrWhiteSpace(subscription.RouteKey)
                    ? subscription.RouteKey
                    : GetEventKey(typeof(T));
            }
            else
            {
                key = string.IsNullOrWhiteSpace(subscription.RouteKey) ? subscription.Queue : subscription.RouteKey;
            }

            DeclareAndBindQueue(queue, key);
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var consumer = new EventingBasicConsumer(_consumerChannel);

            consumer.Received += async (model, ea) => await ReceiveMessage(queue, ea);

            _consumerChannel.BasicConsume(queue, false, consumer);

            SubscriptionManager.AddSubscription<T, TH>(handler, queue);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            SubscriptionManager.Clear();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = _connection.CreateModel();

            //同时只能接受1条消息
            //channel.BasicQos(0, 1, false);
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }
    }
}
