using System;
using System.Reflection;

namespace Acb.Core.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        protected readonly ISubscriptionManager SubscriptionManager;

        protected AbstractEventBus(ISubscriptionManager manager)
        {
            SubscriptionManager = manager ?? new DefaultSubscriptionManager();
        }
        public abstract void Subscribe<T, TH>(Func<TH> handler) where TH : IEventHandler<T>;

        public void Unsubscribe<T, TH>() where TH : IEventHandler<T>
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
        }

        public abstract void Publish(DEvent @event);

        /// <summary> 获取事件的路由键 </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        protected string GetEventKey(Type eventType)
        {
            var attr = eventType.GetCustomAttribute<RouteKeyAttribute>();
            return attr == null ? eventType.Name : attr.Key;
        }

        /// <summary> 获取订阅的队列信息 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected SubscriptionAttribute GetSubscription(Type type)
        {
            var attr = type.GetCustomAttribute<SubscriptionAttribute>() ?? new SubscriptionAttribute();
            if (string.IsNullOrWhiteSpace(attr.Queue))
                attr.Queue = type.FullName;
            return attr;
        }
    }
}
