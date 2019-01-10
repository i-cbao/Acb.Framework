using Acb.Core.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Asb.Spear.Client
{
    public class SpearClient
    {
        private HubConnection _hubConnection;
        private readonly SpearOption _option;
        private readonly ILogger _logger;
        public event Action<object> ConfigChange;

        public SpearClient(SpearOption option)
        {
            _option = option;
            _logger = LogManager.Logger<SpearClient>();
        }

        public async Task Start()
        {
            _hubConnection = new HubConnectionBuilder()
               .WithUrl(_option.HubAddress, opts =>
               {
                   opts.Headers.Add("Authorization",
                       "acb EQS9LTGKzNOHiCn0+8avXJIDCiLW/KtraWRAnl1874nNBAcZ0nPd8KZXUXLC+OnCevPWKVQzju/ZLcSExoq+ps3pwpBGpKtK0ZMOfQoPsu4uvhyRvbuU66eaYaH6w1sPMDLpmxHwBi3C8Mc3bdk4Bi1EC8SYlPct22K+gLG6vAM=");
               })
               .Build();
            //订阅配置更新
            _hubConnection.On<object>("UPDATE", config =>
            {
                _logger.Info(config);
                ConfigChange?.Invoke(config);
            });
            try
            {
                await _hubConnection.StartAsync();
                if (_option.ConfigModules != null && _option.ConfigModules.Length > 0)
                {
                    //订阅配置更新
                    var model = string.IsNullOrWhiteSpace(_option.Mode) ? "dev" : _option.Mode.ToLower();
                    await _hubConnection.SendAsync("Subscript", _option.ConfigModules, model);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
