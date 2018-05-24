using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Core.Modules;
using Acb.Core.Serialize;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            LoadConfigCenter().Wait();
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

        /// <summary> 加载配置中心的配置 </summary>
        private static async Task LoadConfigCenter()
        {
            var uri = "config:uri".Config<string>();
            var apps = "config:application".Config<string>().Split(',');
            if (string.IsNullOrWhiteSpace(uri) || !apps.Any())
                return;
            _logger.Info($"正在加载配置中心[{uri}:{string.Join(",", apps)}]");
            var http = HttpHelper.Instance;
            var configs = new List<string>();
            foreach (var app in apps)
            {
                var url = new Uri(new Uri(uri), $"{app}/{Consts.Mode.ToString().ToLower()}").AbsoluteUri;
                var resp = await http.GetAsync(url);
                var json = await resp.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                    configs.Add(json);
            }
            ConfigHelper.Instance.Build(b =>
            {
                foreach (var config in configs)
                {
                    b.AddJson(config);
                }
            });
        }

        public override void Shutdown()
        {
            HttpHelper.Instance.Dispose();
            base.Shutdown();
        }
    }
}
