using Acb.Core.EventBus;
using Acb.Core.Exceptions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Newtonsoft.Json;
using ons;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.RocketMq
{
    /// <summary> RocketMQ(暂不支持Linux) </summary>
    public class EventBusRocketMq : AbstractEventBus, IDisposable
    {
        private readonly RocketMqConfig _config;
        private Producer _producer;
        private PushConsumer _consumer;
        private static RocketMqLisenter _rocketMqLisenter;

        public EventBusRocketMq(ISubscriptionManager manager, RocketMqConfig config) : base(manager)
        {
            _config = config;
        }

        private ONSFactoryProperty CreateProperty()
        {
            var property = new ONSFactoryProperty();
            property.setFactoryProperty(ONSFactoryProperty.NAMESRV_ADDR, _config.Host);
            if (!string.IsNullOrWhiteSpace(_config.AccsessKey))
            {
                property.setFactoryProperty(ONSFactoryProperty.AccessKey, _config.AccsessKey);
                property.setFactoryProperty(ONSFactoryProperty.SecretKey, _config.SecretKey);
            }

            property.setFactoryProperty(ONSFactoryProperty.PublishTopics, _config.Topic);
            property.setFactoryProperty(ONSFactoryProperty.LogPath, "_logs");
            return property;
        }

        private void CreateProducer()
        {
            if (_producer != null)
                return;
            var property = CreateProperty();
            property.setFactoryProperty(ONSFactoryProperty.ProducerId, _config.ProducerId);
            _producer = ONSFactory.getInstance().createProducer(property);
            _producer.start();
        }

        private void CreateConsumer()
        {
            if (_consumer != null)
                return;
            var property = CreateProperty();
            property.setFactoryProperty(ONSFactoryProperty.ProducerId, _config.ProducerId);
            property.setFactoryProperty(ONSFactoryProperty.ConsumerId, _config.ConsumerId);
            _consumer = ONSFactory.getInstance().createPushConsumer(property);
            _rocketMqLisenter = new RocketMqLisenter();
            _consumer.subscribe(_config.Topic, _config.SubExpression, _rocketMqLisenter);
            _consumer.start();
        }

        public override Task Subscribe<T, TH>(Func<TH> handler)
        {
            if (!SubscriptionManager.HasSubscriptionsForEvent<T>())
            {
                CreateConsumer();
            }
            SubscriptionManager.AddSubscription<T, TH>(handler);
            return Task.CompletedTask;
        }

        public override Task Publish(string key, object @event, long delay = 0, IDictionary<string, object> headers = null)
        {
            CreateProducer();
            var message = JsonConvert.SerializeObject(@event);
            using (var msg = new Message(_config.Topic, key, message))
            {
                msg.setMsgID(IdentityHelper.Guid32);
                msg.setStartDeliverTime(delay);
                var ons = _producer.send(msg);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _producer?.shutdown();
            _consumer?.shutdown();
        }

        public class RocketMqLisenter : MessageListener
        {
            public override ons.Action consume(Message value, ConsumeContext context)
            {
                var body = value.getBody();
                var tag = value.getTag();
                try
                {
                    ProcessEvent(tag, body).GetAwaiter().GetResult();
                    return ons.Action.CommitMessage;
                }
                catch (Exception ex)
                {
                    var logger = LogManager.Logger<RocketMqLisenter>();
                    //非业务异常,可重新入队
                    if (ex is BusiException)
                    {
                        logger.Warn(ex.Message);
                        return ons.Action.CommitMessage;
                    }

                    logger.Error(ex.Message, ex);
                    return ons.Action.ReconsumeLater;

                }
            }
        }
    }
}
