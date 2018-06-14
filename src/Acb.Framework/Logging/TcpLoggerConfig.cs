using Acb.Core.Logging;

namespace Acb.Framework.Logging
{
    public class TcpLoggerConfig
    {
        /// <summary> tcp地址 </summary>
        public string Address { get; set; }

        /// <summary> 端口号 </summary>
        public int Port { get; set; }

        /// <summary> 消息模板 </summary>
        public string Layout { get; set; }

        /// <summary> 日志等级 </summary>
        public LogLevel Level { get; set; } = LogLevel.Error;
    }
}
