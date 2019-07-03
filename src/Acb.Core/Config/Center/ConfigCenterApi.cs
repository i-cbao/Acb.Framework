﻿using Acb.Core.Extensions;
using Acb.Core.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Core.Config.Center
{
    public class ConfigCenterApi
    {
        private const string AuthorizationKey = "Authorization";
        private readonly CenterConfig _config;
        private readonly IDictionary<string, string> _headers;
        private readonly ConcurrentDictionary<string, long> _configVersions;
        private readonly ILogger _logger;

        public ConfigCenterApi(CenterConfig config = null)
        {
            _config = config ?? CenterConfig.Config();
            _headers = new Dictionary<string, string>();
            _configVersions = new ConcurrentDictionary<string, long>();
            _logger = LogManager.Logger<ConfigCenterApi>();
        }

        /// <summary> 统一请求入口 </summary>
        /// <param name="method"></param>
        /// <param name="api"></param>
        /// <param name="data"></param>
        /// <param name="cache">是否开启缓存</param>
        /// <returns></returns>
        private async Task<string> Request(HttpMethod method, string api, object data = null, bool cache = false)
        {
            if (_config == null)
                return string.Empty;
            var url = new Uri(new Uri(_config.Uri), api).AbsoluteUri;
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;
            var client = new HttpClient();
            try
            {
                var req = new HttpRequestMessage(method, url);
                foreach (var header in _headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }

                if (data != null)
                {
                    req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                        "application/json");
                }

                var resp = await client.SendAsync(req);
                var content = await resp.Content.ReadAsStringAsync();
                if (resp.IsSuccessStatusCode)
                {
                    if (cache)
                        url.SetConfig(content);
                    return content;
                }

                if (resp.StatusCode == HttpStatusCode.Forbidden)
                    _headers.Remove(AuthorizationKey);
                _logger.Warn($"ConfigCenter 请求状态异常[{url}]:{resp.StatusCode},{content}");
            }
            catch (Exception ex)
            {
                _logger.Warn($"ConfigCenter 请求异常[{url}]:{ex.Message}");
            }
            finally
            {
                client.Dispose();
            }

            return cache ? url.GetConfig() : string.Empty;
        }

        /// <summary> 登录 </summary>
        /// <returns></returns>
        private async Task CheckLogin()
        {
            if (string.IsNullOrWhiteSpace(_config?.Account) || _headers.ContainsKey(AuthorizationKey))
                return;
            var content = await Request(HttpMethod.Post, "login",
                new { account = _config.Account, password = _config.Password });
            if (string.IsNullOrWhiteSpace(content))
                return;
            var json = JsonConvert.DeserializeObject<dynamic>(content);
            if ((bool)json.ok)
                _headers[AuthorizationKey] = $"acb {json.ticket}";
        }

        /// <summary> 检测版本 </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public async Task<long> CheckVersion(string app)
        {
            await CheckLogin();
            var mode = Consts.Mode.ToString().ToLower();
            var content = await Request(HttpMethod.Get, $"v/{app}/{mode}");
            var version = content.CastTo(-1L);
            if (version <= 0) return version;
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

        /// <summary> 获取配置 </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public async Task<string> GetConfig(string app)
        {
            await CheckLogin();
            var mode = Consts.Mode.ToString().ToLower();
            var content = await Request(HttpMethod.Get, $"{app}/{mode}", cache: true);
            return content;
        }
    }
}
