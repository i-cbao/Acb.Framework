using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Core.EventBus
{
    public interface IEventBus
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
        /// <param name="event">事件</param>
        /// <param name="delay">延迟时间(毫秒)</param>
        /// <param name="headers"></param>
        Task Publish(DEvent @event, long delay = 0, IDictionary<string, object> headers = null);

        /// <summary> 发布 </summary>
        /// <param name="key"></param>
        /// <param name="event"></param>
        /// <param name="delay">延迟时间(毫秒)</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task Publish(string key, object @event, long delay = 0, IDictionary<string, object> headers = null);

        /// <summary> 发布 </summary>
        /// <param name="event">事件</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="headers"></param>
        Task Publish(DEvent @event, TimeSpan delay, IDictionary<string, object> headers = null);

        /// <summary> 发布 </summary>
        /// <param name="key"></param>
        /// <param name="event"></param>
        /// <param name="delay"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task Publish(string key, object @event, TimeSpan delay, IDictionary<string, object> headers = null);

        /// <summary> 发布 </summary>
        /// <param name="event">事件</param>
        /// <param name="delay">延迟时间(秒)</param>
        /// <param name="headers"></param>
        Task Publish(DEvent @event, DateTime delay, IDictionary<string, object> headers = null);

        /// <summary> 发布 </summary>
        /// <param name="key"></param>
        /// <param name="event"></param>
        /// <param name="delay"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task Publish(string key, object @event, DateTime delay, IDictionary<string, object> headers = null);
    }
}
