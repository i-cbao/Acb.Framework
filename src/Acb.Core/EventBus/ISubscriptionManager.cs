using System;
using System.Collections.Generic;
using Acb.Core.Dependency;

namespace Acb.Core.EventBus
{
    /// <summary> 定于管理器 </summary>
    public interface ISubscriptionManager : ISingleDependency
    {
        /// <summary> 是否为空 </summary>
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        /// <summary> 添加订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="handler"></param>
        /// <param name="eventKey"></param>
        void AddSubscription<T, TH>(Func<TH> handler, string eventKey = null)
            where TH : IEventHandler<T>;

        /// <summary> 删除订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void RemoveSubscription<T, TH>(string eventKey = null)
            where TH : IEventHandler<T>;
        /// <summary> 是否已订阅 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<T>();
        /// <summary> 是否已订阅 </summary>
        /// <param name="eventKey"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventKey);
        /// <summary> 获取订阅 </summary>
        /// <param name="eventKey"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventKey);
        /// <summary> 清空订阅 </summary>
        void Clear();
        /// <summary> 获取订阅事件 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<Delegate> GetHandlersForEvent<T>() where T : DEvent;
        /// <summary> 获取订阅事件 </summary>
        /// <param name="eventKey"></param>
        /// <returns></returns>
        IEnumerable<Delegate> GetHandlersForEvent(string eventKey);
    }
}
