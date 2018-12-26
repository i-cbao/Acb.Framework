using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.Core.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        protected readonly ISubscriptionManager SubscriptionManager;

        protected AbstractEventBus(ISubscriptionManager manager)
        {
            SubscriptionManager = manager ?? new DefaultSubscriptionManager();
        }
        public abstract Task Subscribe<T, TH>(Func<TH> handler) where TH : IEventHandler<T>;

        public Task Unsubscribe<T, TH>() where TH : IEventHandler<T>
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
            return Task.CompletedTask;
        }

        public virtual Task Publish(DEvent @event, long delay = 0, IDictionary<string, object> headers = null)
        {
            var key = GetEventKey(@event.GetType());
            return Publish(key, @event, delay, headers);
        }

        public abstract Task Publish(string key, object @event, long delay = 0, IDictionary<string, object> headers = null);

        public Task Publish(DEvent @event, TimeSpan delay, IDictionary<string, object> headers = null)
        {
            return Publish(@event, (long)delay.TotalMilliseconds, headers);
        }

        public Task Publish(string key, object @event, TimeSpan delay, IDictionary<string, object> headers = null)
        {
            return Publish(key, @event, (long)delay.TotalMilliseconds, headers);
        }

        public Task Publish(DEvent @event, DateTime delay, IDictionary<string, object> headers = null)
        {
            return delay < Clock.Now ? Task.CompletedTask : Publish(@event, delay - Clock.Now, headers);
        }

        public Task Publish(string key, object @event, DateTime delay, IDictionary<string, object> headers = null)
        {
            return delay < Clock.Now ? Task.CompletedTask : Publish(key, @event, delay - Clock.Now, headers);
        }

        /// <summary> 获取事件的路由键 </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        protected string GetEventKey(Type eventType)
        {
            var attr = eventType.GetCustomAttribute<RouteKeyAttribute>();
            return attr == null ? eventType.Name : attr.Key;
        }

        protected static async Task ProcessEvent(string eventName, string message)
        {
            var manager = CurrentIocManager.Resolve<ISubscriptionManager>();
            if (manager.HasSubscriptionsForEvent(eventName))
            {
                var eventType = manager.GetEventTypeByName(eventName);
                var integrationEvent = eventType.IsSimpleType()
                    ? message.CastTo(eventType)
                    : JsonConvert.DeserializeObject(message, eventType);
                var handlers = manager.GetHandlersForEvent(eventName);

                foreach (var handlerfactory in handlers)
                {
                    var handler = handlerfactory.DynamicInvoke();
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    var method = concreteType.GetMethod("Handle");
                    if (method == null)
                        continue;
                    await (Task)method.Invoke(handler, new[] { integrationEvent });
                }
            }
        }
    }
}
