using Acb.Core.Timing;
using Newtonsoft.Json;
using System;

namespace Acb.Core.Monitor
{
    public class MonitorData
    {
        /// <summary> 服务名称 </summary>
        public string Service { get; set; }

        /// <summary> URL地址 </summary>
        public string Url { get; set; }

        /// <summary> 请求方法 </summary>
        public string Method { get; set; }

        /// <summary> 请求数据 </summary>
        public string Data { get; set; }

        /// <summary> 来源 </summary>
        public string Referer { get; set; }

        /// <summary> 客户端IP </summary>
        public string ClientIp { get; set; }

        /// <summary> 客户端信息 </summary>
        public string UserAgent { get; set; }

        /// <summary> 开始时间 </summary>
        public DateTime BeginTime { get; set; }

        /// <summary> 完成时间 </summary>
        public DateTime CompleteTime { get; set; }

        /// <summary> 耗时(毫秒) </summary>
        public long Time => (CompleteTime - BeginTime).Milliseconds;

        /// <summary> 状态码 </summary>
        public int Code { get; set; } = 200;

        /// <summary> 执行结果 </summary>
        public string Result { get; set; }

        public MonitorData()
        {
            BeginTime = Clock.Now;
            ClientIp = AcbHttpContext.ClientIp;
            UserAgent = AcbHttpContext.UserAgent;
        }

        public MonitorData(string service) : this()
        {
            Service = service;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
