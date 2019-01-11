using Acb.RabbitMq.Options;
using System;

namespace Acb.RabbitMq
{
    /// <inheritdoc />
    /// <summary> 订阅属性 </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscriptionAttribute : Attribute
    {
        /// <summary> 订阅配置 </summary>
        public RabbitMqSubscribeOption Option { get; set; }

        /// <summary> 订阅属性 </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="routeKey">路由键,默认为事件属性的RouteKey</param>
        public SubscriptionAttribute(string queue, string routeKey = null)
        {
            Queue = queue;
            RouteKey = routeKey;
            Option = new RabbitMqSubscribeOption(queue, routeKey);
        }

        /// <summary> 队列名称 </summary>
        public string Queue
        {
            get => Option.Queue;
            set => Option.Queue = value;
        }

        /// <summary> 路由键 </summary>
        public string RouteKey
        {
            get => Option.RouteKey;
            set => Option.RouteKey = value;
        }
    }
}
