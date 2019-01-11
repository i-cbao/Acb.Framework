using Acb.Core.EventBus;
using Acb.Core.EventBus.Options;
using Acb.Core.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Acb.Redis
{
    public class EventBusRedis : AbstractEventBus, IDisposable
    {
        private readonly ISubscriber _subscriber;
        private const string EventBusName = "eventBus";
        private readonly ILogger _logger;
        public EventBusRedis(ISubscribeManager manager, IMessageCodec codec, string configName = null) : base(manager, codec)
        {
            configName = string.IsNullOrWhiteSpace(configName) ? EventBusName : configName;
            _subscriber = RedisManager.Instance.GetSubscriber(configName);
            _logger = LogManager.Logger<EventBusRedis>();
        }

        public override Task Subscribe<T, TH>(Func<TH> handler, SubscribeOption option = null)
        {
            if (!SubscriptionManager.HasSubscriptionsForEvent<T>())
            {
                var key = typeof(T).GetRouteKey();
                _subscriber.SubscribeAsync(key,
                    async (channel, value) =>
                    {
                        byte[] message = value;
                        //var message = Encoding.UTF8.GetBytes(value);
                        try
                        {
                            await SubscriptionManager.ProcessEvent(channel, message);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message, ex);
                        }
                    });
            }

            SubscriptionManager.AddSubscription<T, TH>(handler);
            return Task.CompletedTask;
        }

        public override Task Publish(string key, byte[] message, PublishOption option = null)
        {
            //:todo delay
            return _subscriber.PublishAsync(key, message);
        }

        public void Dispose()
        {
            _subscriber.UnsubscribeAll();
        }
    }
}
