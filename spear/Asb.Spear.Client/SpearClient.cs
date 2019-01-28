using Acb.Core.Config;
using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Asb.Spear.Client
{
    public class SpearClient : IDisposable
    {
        private const string AuthorizeKey = "Authorization";
        private const string ProjectKey = "project";

        private HubConnection _configHub;
        private HubConnection _jobHub;
        private Timer _retryTimer;
        private readonly SpearOption _option;
        private readonly ILogger _logger;
        private readonly IDictionary<string, string> _headers;
        public event Action<object> ConfigChange;

        public SpearClient(SpearOption option)
        {
            _option = option;
            _logger = LogManager.Logger<SpearClient>();
            _headers = new Dictionary<string, string>();
        }

        private async Task LoadTicket()
        {
            if (_option.Secret.IsNullOrEmpty())
            {
                if (_headers.ContainsKey(ProjectKey)) return;
                _headers.Add(ProjectKey, _option.Code);
            }
            else
            {
                if (_headers.ContainsKey(AuthorizeKey))
                    return;
                _logger.Info("正在加载配置中心令牌");
                try
                {
                    var loginUrl = new Uri(new Uri(_option.Url), "api/account/login").AbsoluteUri;
                    var loginResp = await HttpHelper.Instance.PostAsync(loginUrl,
                        new { account = _option.Code, password = _option.Secret });
                    var data = await loginResp.Content.ReadAsStringAsync();
                    if (loginResp.IsSuccessStatusCode)
                    {
                        var json = new
                        {
                            Status = false,
                            Data = string.Empty
                        };
                        json = JsonConvert.DeserializeAnonymousType(data, json);
                        if (json.Status)
                            _headers[AuthorizeKey] = $"acb {json.Data}";
                    }
                    else
                    {
                        _logger.Info(data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"{_option.Url}:{ex.Message}", ex);
                }
            }
        }

        private HubConnection Connect(SpearType type)
        {
            var url = $"{_option.Url}/{type.GetText()}";
            return new HubConnectionBuilder()
                .WithUrl(url, async opts =>
                {
                    await LoadTicket();
                    foreach (var header in _headers)
                    {
                        opts.Headers.Add(header.Key, header.Value);
                    }
                })
                .Build();
        }

        private Task ConnectConfig(ConfigOption option)
        {
            async Task Start()
            {
                try
                {
                    await _configHub.StartAsync();
                    if (option.ConfigModules != null && option.ConfigModules.Length > 0)
                    {
                        //订阅配置更新
                        var model = string.IsNullOrWhiteSpace(option.Mode) ? "dev" : option.Mode.ToLower();
                        await _configHub.SendAsync("Subscript", option.ConfigModules, model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"{_option.Url}:{ex.Message}", ex);
                }
            };

            _retryTimer = new Timer(async obj =>
            {
                if (_configHub.State == HubConnectionState.Connected)
                {
                    _retryTimer.Dispose();
                    return;
                }
                _logger.Info("retry connect");
                await Start();
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public async Task StartConfig(ConfigOption option)
        {
            _configHub = Connect(SpearType.Config);
            var provider = new SpearConfigProvider();
            ConfigHelper.Instance.Build(b => b.Add(provider));
            //订阅配置更新
            _configHub.On<IDictionary<string, object>>("UPDATE", configs =>
              {
                  _logger.Info(configs);
                  foreach (var config in configs)
                  {
                      provider.LoadConfig(config.Key, config.Value);
                      ConfigChange?.Invoke(config);
                  }
              });
            _configHub.Closed += async ex =>
            {
                _logger.Info("connect closed");
                await ConnectConfig(option);
            };
            await ConnectConfig(option);
        }

        public async Task StartJob(JobOption option)
        {
            _jobHub = Connect(SpearType.Jobs);
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                if (_configHub != null)
                    await _configHub.DisposeAsync();
                if (_jobHub != null)
                    await _jobHub.DisposeAsync();
            });
        }
    }
}
