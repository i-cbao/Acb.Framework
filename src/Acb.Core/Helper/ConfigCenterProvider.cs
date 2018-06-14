using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acb.Core.Helper
{
    internal class ConfigCenterProvider : ConfigurationProvider, IConfigurationSource
    {
        private class ConfigCenter
        {
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

        private ConfigCenter _config;
        private IDictionary<string, string> _headers;
        private readonly ILogger _logger;
        private const string ArrayPattern = @"(\[[0-9]+\])*$";

        public ConfigCenterProvider()
        {
            _logger = LogManager.Logger<ConfigCenterProvider>();
        }

        private async Task LoadTicket()
        {
            if (!string.IsNullOrWhiteSpace(_config.Account))
            {
                _logger.Info("正在加载配置中心令牌");
                try
                {
                    var loginUrl = new Uri(new Uri(_config.Uri), "login").AbsoluteUri;
                    var loginResp = await HttpHelper.Instance.PostAsync(loginUrl,
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
                        _logger.Info(data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        private async Task LoadConfig(bool reload = false)
        {
            Data.Clear();
            if (string.IsNullOrWhiteSpace(_config.Uri) || string.IsNullOrWhiteSpace(_config.Application))
                return;
            var parser = new JsonConfigurationParser();
            var apps = _config.Application.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            var act = reload ? "更新" : "加载";
            _logger.Info($"正在{act}配置中心[{_config.Uri}:{Consts.Mode.ToString().ToLower()}:{string.Join(",", apps)}]");
            foreach (var app in apps)
            {
                try
                {
                    var path = $"{app}/{Consts.Mode.ToString().ToLower()}";
                    var url = new Uri(new Uri(_config.Uri), path).AbsoluteUri;
                    var resp = await HttpHelper.Instance.RequestAsync(HttpMethod.Get,
                        new HttpRequest(url) { Headers = _headers });
                    var json = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                    {
                        _logger.Warn($"{path}:{json}");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(json))
                        continue;
                    var t = parser.Parse(json);
                    foreach (var key in t.Keys)
                    {
                        var dKey = ConvertKey(key);
                        if (string.IsNullOrWhiteSpace(dKey))
                            continue;
                        Data[dKey] = t[key];
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        protected internal virtual string ConvertKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            var split = key.Split('.');
            var sb = new StringBuilder();
            foreach (var part in split)
            {
                var keyPart = ConvertArrayKey(part);
                sb.Append(keyPart);
                sb.Append(ConfigurationPath.KeyDelimiter);
            }

            return sb.ToString(0, sb.Length - 1);
        }

        protected internal virtual string ConvertArrayKey(string key)
        {
            return Regex.Replace(key, ArrayPattern, (match) =>
            {
                var result = match.Value.Replace("[", ":").Replace("]", string.Empty);
                return result;
            });
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            _headers = new Dictionary<string, string>();
            _config = "config".Config<ConfigCenter>() ?? new ConfigCenter();
            LoadTicket().Wait();
            StartRefresh();
            return this;
        }

        private void StartRefresh()
        {
            _logger.Info($"refresh:{_config.Interval}");
            if (_config.Interval > 0)
            {
                ConfigCenterRefresh.Instance.Start(_config.Interval, this);
            }
            else
            {
                ConfigCenterRefresh.Instance.Stop();
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
            _config = "config".Config<ConfigCenter>() ?? new ConfigCenter();
            LoadTicket().Wait();
            StartRefresh();
        }
    }
}
