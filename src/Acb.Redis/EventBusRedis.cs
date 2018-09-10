using Acb.Core.EventBus;
using Acb.Core.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Redis
{
    public class EventBusRedis : AbstractEventBus, IDisposable
    {
        private readonly ISubscriber _subscriber;
        private const string EventBusName = "eventBus";
        private readonly ILogger _logger;
        public EventBusRedis(ISubscriptionManager manager, string configName = null) : base(manager)
        {
            configName = string.IsNullOrWhiteSpace(configName) ? EventBusName : configName;
            _subscriber = RedisManager.Instance.GetSubscriber(configName);
            _logger = LogManager.Logger<EventBusRedis>();
        }

        public override Task Subscribe<T, TH>(Func<TH> handler)
        {
            if (!SubscriptionManager.HasSubscriptionsForEvent<T>())
            {
                var key = GetEventKey(typeof(T));
                _subscriber.SubscribeAsync(key,
                    async (channel, value) =>
                    {
                        var message = Encoding.UTF8.GetString(value);
                        try
                        {
                            await ProcessEvent(channel, message);
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

        public override Task Publish(string key, object @event, long delay = 0, IDictionary<string, object> headers = null)
        {
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            return _subscriber.PublishAsync(key, body);
        }

        public void Dispose()
        {
            _subscriber.UnsubscribeAll();
        }
    }
}
