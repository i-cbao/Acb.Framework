using System.Collections.Generic;

namespace Acb.Spear.Message
{
    /// <summary> 调用消息 </summary>
    public class InvokeMessage
    {
        /// <summary> 服务Id </summary>
        public string ServiceId { get; set; }
        /// <summary> 服务参数 </summary>
        public IDictionary<string, object> Parameters { get; set; }
    }
}
