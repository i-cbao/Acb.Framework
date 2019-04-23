using Acb.Core.EventBus.Options;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Acb.Core.Message;

namespace Acb.Core.EventBus
{
    /// <summary> 事件总线 </summary>
    public interface IEventBus
    {
        /// <summary> 编解码器 </summary>
        IMessageCodec Codec { get; }

        /// <summary> 订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="handler"></param>
        /// <param name="option"></param>
        Task Subscribe<T, TH>(Func<TH> handler, SubscribeOption option = null)
            where TH : IEventHandler<T>;

        /// <summary> 取消订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        Task Unsubscribe<T, TH>()
            where TH : IEventHandler<T>;

        /// <summary> 发布 </summary>
        /// <param name="key">事件</param>
        /// <param name="message"></param>
        /// <param name="option"></param>
        Task Publish(string key, object message, PublishOption option = null);
    }

    /// <summary> 扩展 </summary>
    public static class EventBusExtensions
    {
        /// <summary> 获取事件的路由键 </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static string GetRouteKey(this MemberInfo eventType)
        {
            var attr = eventType.GetCustomAttribute<RouteKeyAttribute>();
            return attr == null ? eventType.Name : attr.Key;
        }

        /// <summary> 发布 </summary>
        /// <param name="eventBus"></param>
        /// <param name="event">事件</param>
        /// <param name="option"></param>
        public static Task Publish(this IEventBus eventBus, DEvent @event, PublishOption option = null)
        {
            var key = @event.GetType().GetRouteKey();
            return eventBus.Publish(key, @event, option);
        }
    }
}
