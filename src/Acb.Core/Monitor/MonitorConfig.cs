using Acb.Core.Extensions;
using System.Collections.Generic;

namespace Acb.Core.Monitor
{
    public class MonitorConfig
    {
        private const string MonitorEnableEnv = "MONITOR_ENABLE";
        private const string MonitorConfigKey = "monitor";

        /// <summary> 是否开启监控 </summary>
        public bool Enable { get; set; }

        /// <summary> 模块配置 </summary>
        public IDictionary<string, bool> Modules { get; set; }

        public MonitorConfig()
        {
            Modules = new Dictionary<string, bool>();
        }

        public static MonitorConfig Config(string name = null)
        {
            var configName = string.IsNullOrWhiteSpace(name) ? MonitorConfigKey : name;
            var config = configName.Config<MonitorConfig>() ?? new MonitorConfig();
            //判断环境变量
            var enable = MonitorEnableEnv.Env(false);
            if (enable) config.Enable = true;
            return config;
        }
    }
}
