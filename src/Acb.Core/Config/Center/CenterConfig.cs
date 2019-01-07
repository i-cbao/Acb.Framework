using Acb.Core.Extensions;
using System;

namespace Acb.Core.Config.Center
{
    /// <summary> 配置中心配置 </summary>
    [Serializable]
    public class CenterConfig
    {
        private const string DefaultName = "config";

        public static CenterConfig Config(string configName = null)
        {
            configName = string.IsNullOrWhiteSpace(configName) ? DefaultName : configName;
            return configName.Config<CenterConfig>() ?? new CenterConfig();
        }
        /// <summary> 配置中心url </summary>
        public string Uri { get; set; }
        /// <summary> 需要加载的配置应用 </summary>
        public string Application { get; set; }
        /// <summary> 账号 </summary>
        public string Account { get; set; }
        /// <summary> 密码 </summary>
        public string Password { get; set; }
        /// <summary> 更新时间(秒) </summary>
        public int Interval { get; set; }
    }
}
