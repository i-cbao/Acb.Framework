using Acb.Core.Dependency;
using System;

namespace Acb.Core.EventBus
{
    public interface IEventBus : ISingleDependency
    {
        /// <summary> 订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="handler"></param>
        void Subscribe<T, TH>(Func<TH> handler)
            where TH : IEventHandler<T>;

        /// <summary> 取消订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void Unsubscribe<T, TH>()
            where TH : IEventHandler<T>;

        /// <summary> 发布 </summary>
        /// <param name="event"></param>
        void Publish(DEvent @event);
    }
}
