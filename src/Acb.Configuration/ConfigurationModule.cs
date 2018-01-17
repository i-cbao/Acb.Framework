using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Modules;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace Acb.Configuration
{
    [DependsOn(typeof(CoreModule))]
    public class ConfigurationModule : DModule
    {
        public override void Initialize()
        {
            var uri = "config:uri".Config<string>();
            var app = "config:application".Config<string>();
            var helper = ConfigHelper.Instance;
            if (uri.IsUrl() && !string.IsNullOrWhiteSpace(app))
            {
                var provider = new ConfigServerConfigurationProvider();
                helper.Build(b => b.Add(provider));
                helper.ConfigChanged += obj =>
                {
                    var config = obj as IConfigurationRoot;
                    var mode = config.GetValue<string>("mode");
                    if (config == null || string.IsNullOrWhiteSpace(mode) || config.GetSection("config") == null)
                        return;
                    provider.Reload(helper.Config);
                    if (provider.Settings.RefreshEnable)
                    {
                        RefreshHelper.Start(provider.Settings.RefreshInterval);
                    }
                    else
                    {
                        RefreshHelper.Stop();
                    }
                };
                if (provider.Settings.RefreshEnable)
                    RefreshHelper.Start(provider.Settings.RefreshInterval);
            }

            var configPath = "configPath".Config<string>();
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                configPath = Path.Combine(Directory.GetCurrentDirectory(), configPath);
                if (Directory.Exists(configPath))
                {
                    var jsons = Directory.GetFiles(configPath, "*.json");
                    if (jsons != null && jsons.Any())
                    {
                        helper.Build(b =>
                        {
                            foreach (var json in jsons)
                            {
                                b.AddJsonFile(json);
                            }
                        });
                    }
                }
            }

            base.Initialize();
        }

        public override void Shutdown()
        {
            RefreshHelper.Stop();
            base.Shutdown();
        }
    }
}
