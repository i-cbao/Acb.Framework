using System;

namespace Acb.EventBus
{
    /// <inheritdoc />
    /// <summary> 订阅属性 </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscriptionAttribute : Attribute
    {
        public SubscriptionAttribute(string queue = null)
        {
            Queue = queue;
        }
        /// <summary> 队列名称 </summary>
        public string Queue { get; set; }
        /// <summary> 持久化(默认：true) </summary>
        public bool Durable { get; set; } = true;

        /// <summary> 自动删除 </summary>
        public bool AutoDelete { get; set; }
        /// <summary> 专用的 </summary>
        public bool Exclusive { get; set; }
    }
}
