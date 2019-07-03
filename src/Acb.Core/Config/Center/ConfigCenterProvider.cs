using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Core.Config.Center
{
    /// <summary> 中心配置提供者 </summary>
    internal class ConfigCenterProvider : DConfigProvider, IConfigurationSource
    {
        private readonly bool _reload;
        private CenterConfig _config;
        private readonly ConfigCenterApi _configApi;

        public ConfigCenterProvider(CenterConfig config, bool reload = false)
        {
            _config = config;
            _configApi = new ConfigCenterApi(config);
            _reload = reload;
        }

        /// <summary> 加载配置 </summary>
        /// <param name="reload">使用重新加载</param>
        /// <returns></returns>
        private async Task LoadConfig(bool reload = false)
        {
            if (!reload)
                Data.Clear();
            if (string.IsNullOrWhiteSpace(_config.Uri) || string.IsNullOrWhiteSpace(_config.Application))
                return;
            var mode = Consts.Mode.ToString().ToLower();
            var apps = _config.Application.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            var act = reload ? "更新" : "加载";
            foreach (var app in apps)
            {
                try
                {
                    var version = await _configApi.CheckVersion(app);
                    if (version == 0)
                        continue;
                    Logger.Info($"正在{act}配置中心[{_config.Uri}:{mode}:{app}_{version}]");
                    var json = await _configApi.GetConfig(app);
                    if (!string.IsNullOrWhiteSpace(json))
                        LoadJson(json);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            StartRefresh();
            return this;
        }

        private void StartRefresh()
        {
            var refresh = CurrentIocManager.Resolve<ConfigCenterRefresh>();
            if (_config.Interval > 0)
            {
                Logger.Info($"refresh:{_config.Interval}");
                refresh.Start(_config.Interval, this);
            }
            else
            {
                refresh.Stop();
            }
        }

        public override void Load()
        {
            LoadConfig().SyncRun();
        }

        internal void Reload(object state = null)
        {
            if (state == null)
            {
                LoadConfig(true).SyncRun();
                return;
            }
            if (!(state is IConfigurationRoot config) || config.Providers.All(t => t is ConfigCenterProvider))
                return;
            if (_reload)
                _config = CenterConfig.Config();
            StartRefresh();
        }
    }
}
