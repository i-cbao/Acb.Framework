using Acb.Core.Extensions;

namespace Acb.Core.Logging.Remote
{
    public class RemoteLoggerConfig
    {
        private const string ConfigKey = "tcpLogger";

        /// <summary> tcp地址 </summary>
        public string Address { get; set; }

        /// <summary> 端口号 </summary>
        public int Port { get; set; }

        ///// <SUMMARY> 消息模板 </SUMMARY>
        //PUBLIC STRING LAYOUT { GET; SET; }

        /// <summary> 日志等级 </summary>
        public LogLevel Level { get; set; } = LogLevel.Error;

        public static RemoteLoggerConfig Config(string key = null)
        {
            return (string.IsNullOrWhiteSpace(key) ? ConfigKey : key).Config<RemoteLoggerConfig>();
        }
    }
}
