using Acb.Core.EventBus;
using Acb.Core.Exceptions;
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

        public EventBusRabbitMq(IRabbitMqConnection connection, ISubscriptionManager subsManager, RabbitMqConfig config) : base(subsManager)
        {
            _brokerName = config.Broker;
            _connection =
                connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = LogManager.Logger<EventBusRabbitMq>();
            _consumerChannel = CreateConsumerChannel();
            SubscriptionManager.OnEventRemoved += SubsManager_OnEventRemoved;
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
                _consumerChannel.Close();
            }
        }

        public override void Publish(DEvent @event)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.Warn(ex.ToString());
                });
            policy.Execute(() =>
            {
                using (var channel = _connection.CreateModel())
                {
                    var key = GetEventKey(@event.GetType());
                    //声明Exchange
                    channel.ExchangeDeclare(_brokerName, ExchangeType.Topic, true);
                    var message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);
                    var prop = channel.CreateBasicProperties();
                    prop.DeliveryMode = 2;
                    channel.BasicPublish(_brokerName, key, prop, body);
                }
            });
        }

        public override void Subscribe<T, TH>(Func<TH> handler)
        {
            var key = GetEventKey(typeof(T));
            var subscription = GetSubscription(typeof(TH));
            var queue = subscription.Queue;
            var containsKey = SubscriptionManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                if (!_connection.IsConnected)
                {
                    _connection.TryConnect();
                }
                var consumer = new EventingBasicConsumer(_consumerChannel);
                consumer.Received += async (model, ea) =>
                {
                    var name = ea.RoutingKey;
                    var message = Encoding.UTF8.GetString(ea.Body);
                    try
                    {
                        await ProcessEvent(name, message);
                        _consumerChannel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        //非业务异常,可重新入队
                        _consumerChannel.BasicNack(ea.DeliveryTag, false, !(ex is BusiException));
                        _logger.Error(ex.Message, ex);
                    }
                };

                var dlxExchange = $"{_brokerName}_dlx";
                var dlxQueue = $"{queue}_dlx";
                var args = new Dictionary<string, object>
                {
                    {"x-dead-letter-exchange", dlxExchange},
                    {"x-dead-letter-routing-key", key}
                };

                //死信队列
                //1.消息被拒绝，并且设置ReQueue参数false；
                //2.消息过期；
                //3.队列打到最大长度；
                _consumerChannel.ExchangeDeclare(dlxExchange, ExchangeType.Direct, true);
                //声明死信队列
                _consumerChannel.QueueDeclare(dlxQueue, true, false, false, args);
                _consumerChannel.QueueBind(dlxQueue, dlxExchange, key, null);


                _consumerChannel.ExchangeDeclare(_brokerName, ExchangeType.Topic, true);
                //声明队列
                _consumerChannel.QueueDeclare(queue, subscription.Durable, subscription.Exclusive, subscription.AutoDelete, args);
                _consumerChannel.QueueBind(queue, _brokerName, key, null);

                _consumerChannel.BasicConsume(queue, false, consumer);
            }

            SubscriptionManager.AddSubscription<T, TH>(handler);

        }

        private static Func<IEventHandler> FindHandlerByType(Type handlerType, IEnumerable<Func<IEventHandler>> handlers)
        {
            foreach (var func in handlers)
            {
                if (func.GetMethodInfo().ReturnType == handlerType)
                {
                    return func;
                }
            }

            return null;
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

        private async Task ProcessEvent(string eventName, string message)
        {

            if (SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                var eventType = SubscriptionManager.GetEventTypeByName(eventName);
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                var handlers = SubscriptionManager.GetHandlersForEvent(eventName);

                foreach (var handlerfactory in handlers)
                {
                    var handler = handlerfactory.DynamicInvoke();
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
    }
}
