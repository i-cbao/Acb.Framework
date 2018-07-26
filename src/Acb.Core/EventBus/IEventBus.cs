using Acb.Core.Dependency;
using System;
using System.Threading.Tasks;

namespace Acb.Core.EventBus
{
    public interface IEventBus : ISingleDependency
    {
        /// <summary> 订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="handler"></param>
        Task Subscribe<T, TH>(Func<TH> handler)
            where TH : IEventHandler<T>;

        /// <summary> 取消订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        Task Unsubscribe<T, TH>()
            where TH : IEventHandler<T>;

        /// <summary> 发布 </summary>
        /// <param name="event"></param>
        Task Publish(DEvent @event);

        /// <summary> 发布 </summary>
        /// <param name="key"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        Task Publish(string key, object @event);
    }
}
