using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acb.Core.Config.Center
{
    /// <summary> 中心配置提供者 </summary>
    internal class ConfigCenterProvider : DConfigProvider, IConfigurationSource
    {
        private CenterConfig _config;
        private readonly bool _reload;
        private IDictionary<string, string> _headers;
        private readonly HttpHelper _httpHelper;

        private readonly ConcurrentDictionary<string, long> _configVersions;

        public ConfigCenterProvider(CenterConfig config, bool reload = false)
        {
            _httpHelper = HttpHelper.Instance;
            _configVersions = new ConcurrentDictionary<string, long>();
            _config = config;
            _reload = reload;
        }

        private async Task LoadTicket()
        {
            if (string.IsNullOrWhiteSpace(_config.Account))
                return;
            Logger.Info("正在加载配置中心令牌");
            try
            {
                var loginUrl = new Uri(new Uri(_config.Uri), "login").AbsoluteUri;
                var loginResp = await _httpHelper.PostAsync(loginUrl,
                    new { account = _config.Account, password = _config.Password });
                var data = await loginResp.Content.ReadAsStringAsync();
                if (loginResp.IsSuccessStatusCode)
                {
                    var json = JsonConvert.DeserializeObject<dynamic>(data);
                    if ((bool)json.ok)
                        _headers["Authorization"] = $"acb {json.ticket}";
                }
                else
                {
                    Logger.Info(data);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary> 检测版本号 </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task<long> CheckVersion(string app)
        {
            var mode = Consts.Mode.ToString().ToLower();
            Logger.Debug($"正在检测配置中心版本[{_config.Uri}:{mode}:{app}]");
            var versionPath = $"v/{app}/{mode}";
            var versionUrl = new Uri(new Uri(_config.Uri), versionPath).AbsoluteUri;
            var versionResp =
                await _httpHelper.RequestAsync(HttpMethod.Get, new HttpRequest(versionUrl) { Headers = _headers });
            var version = (await versionResp.Content.ReadAsStringAsync()).CastTo(0L);
            if (_configVersions.ContainsKey(app))
            {
                var change = version != _configVersions[app];
                if (change)
                    _configVersions[app] = version;
                return change ? version : 0;
            }

            _configVersions.TryAdd(app, version);
            return version;
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
                    var version = await CheckVersion(app);
                    if (version <= 0)
                        continue;
                    Logger.Info($"正在{act}配置中心[{_config.Uri}:{mode}:{app}_{version}]");
                    var path = $"{app}/{Consts.Mode.ToString().ToLower()}";
                    var url = new Uri(new Uri(_config.Uri), path).AbsoluteUri;
                    var resp = await _httpHelper.RequestAsync(HttpMethod.Get,
                        new HttpRequest(url) { Headers = _headers });
                    var json = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                    {
                        Logger.Warn($"{path}:{json}");
                        continue;
                    }

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
            _headers = new Dictionary<string, string>();
            _config = _config ?? CenterConfig.Config();
            LoadTicket().Wait();
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
            LoadConfig().Wait();
        }

        internal void Reload(object state = null)
        {
            if (state == null)
            {
                LoadConfig(true).Wait();
                return;
            }
            if (!(state is IConfigurationRoot config) || config.Providers.All(t => t is ConfigCenterProvider))
                return;
            if (_reload)
                _config = CenterConfig.Config();
            LoadTicket().Wait();
            StartRefresh();
        }
    }
}
