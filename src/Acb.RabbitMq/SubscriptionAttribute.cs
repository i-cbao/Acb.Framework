using System;

namespace Acb.RabbitMq
{
    /// <inheritdoc />
    /// <summary> 订阅属性 </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscriptionAttribute : Attribute
    {
        /// <summary> 订阅属性 </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="routeKey">路由键,默认为事件属性的RouteKey</param>
        public SubscriptionAttribute(string queue, string routeKey = null)
        {
            Queue = queue;
            RouteKey = routeKey;
        }
        /// <summary> 队列名称 </summary>
        public string Queue { get; set; }
        /// <summary> 路由键 </summary>
        public string RouteKey { get; set; }
    }
}
