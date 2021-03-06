﻿using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Acb.EventBus.Redis
{
    public class EventBusRedis : AbstractEventBus, IDisposable
    {
        private readonly ISubscriber _subscriber;
        private const string EventBusName = "eventBus";
        public EventBusRedis(ISubscriptionManager manager, IRedisConnection connection) : base(manager)
        {
            _subscriber = connection.GetSubscriber();
        }

        public override Task Subscribe<T, TH>(Func<TH> handler)
        {
            if (!SubscriptionManager.HasSubscriptionsForEvent<T>())
            {
                var key = GetEventKey(typeof(T));
                //var subscription = GetSubscription(typeof(TH));
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
                        }
                    });
            }

            SubscriptionManager.AddSubscription<T, TH>(handler);
            return Task.CompletedTask;
        }

        public override Task Publish(string key, object @event)
        {
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            return _subscriber.PublishAsync(key, body);
        }

        public void Dispose()
        {
            _subscriber.UnsubscribeAll();
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
