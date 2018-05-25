using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Core.Modules;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Acb.Core
{
    /// <summary> 初始化模块 </summary>
    public class CoreModule : DModule
    {
        private static ILogger _logger;
        /// <inheritdoc />
        /// <summary> 初始化 </summary>
        public override void Initialize()
        {
            CurrentIocManager.IocManager = IocManager;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _logger = LogManager.Logger<CoreModule>();
            LoadLocalConfig();
            var provider = new ConfigCenterProvider();
            ConfigHelper.Instance.Build(b => b.Add(provider));
            ConfigHelper.Instance.ConfigChanged += provider.Reload;

            base.Initialize();
        }

        /// <summary> 加载本地配置 </summary>
        private static void LoadLocalConfig()
        {
            var configHelper = ConfigHelper.Instance;
            //本地文件配置
            var configPath = "configPath".Config<string>();
            if (string.IsNullOrWhiteSpace(configPath))
                return;
            configPath = Path.Combine(Directory.GetCurrentDirectory(), configPath);
            _logger.Info($"正在加载本地配置[{configPath}]");
            if (!Directory.Exists(configPath))
                return;
            var jsons = Directory.GetFiles(configPath, "*.json");
            if (jsons != null && jsons.Any())
            {
                configHelper.Build(b =>
                {
                    foreach (var json in jsons)
                    {
                        b.AddJsonFile(json, false, true);
                    }
                });
            }
        }

        public override void Shutdown()
        {
            HttpHelper.Instance.Dispose();
            base.Shutdown();
        }
    }
}
