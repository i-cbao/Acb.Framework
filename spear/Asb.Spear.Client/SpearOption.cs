using Acb.Core.Extensions;

namespace Asb.Spear.Client
{
    public class SpearOption
    {
        /// <summary> Host </summary>
        public string Host { get; set; }

        /// <summary> Port </summary>
        public int Port { get; set; }

        /// <summary> 项目编码 </summary>
        public string Code { get; set; }
        /// <summary> 项目密钥 </summary>
        public string Secret { get; set; }

        public string Url
        {
            get
            {
                var host = Host;
                if (!host.IsUrl())
                    host = $"http://{host}";
                return Port == 80 ? host : $"{host}:{Port}";
            }
        }
    }

    public class ConfigOption
    {
        /// <summary> 开发模式：dev/test/ready/prod </summary>
        public string Mode { get; set; }

        /// <summary> 配置模块 </summary>
        public string[] ConfigModules { get; set; }
    }

    public class JobOption
    {

    }
}
