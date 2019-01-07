using Acb.Core;
using Acb.Core.Logging;
using Acb.Spear.Domain;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Hubs
{
    /// <summary> 配置中心总线 </summary>
    public class ConfigHub : Hub
    {
        private const string CodeKey = "code";
        private readonly ILogger _logger;

        public ConfigHub()
        {
            _logger = LogManager.Logger<ConfigHub>();
        }

        /// <summary> 订阅配置 </summary>
        /// <param name="modes">模块</param>
        /// <param name="env">环境模式</param>
        /// <returns></returns>
        public async Task Subscript(string[] modes, string env)
        {
            _logger.Info($"hub:{Context.ConnectionId} Subscript {env} - {string.Join(',', modes)}");
            Context.Items.TryGetValue(CodeKey, out var code);
            if (code == null || string.IsNullOrWhiteSpace(code.ToString()))
                return;
            foreach (var mode in modes)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"{code}_{mode}_{env}");
            }
        }

        /// <summary> 取消订阅配置 </summary>
        /// <param name="modes">模块</param>
        /// <param name="env">环境模式</param>
        /// <returns></returns>
        public async Task UnSubscript(string[] modes, string env)
        {
            _logger.Info($"hub:{Context.ConnectionId} UnSubscript {env} - {string.Join(',', modes)}");
            Context.Items.TryGetValue(CodeKey, out var code);
            if (code == null || string.IsNullOrWhiteSpace(code.ToString()))
                return;
            foreach (var mode in modes)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{code}_{mode}_{env}");
            }
        }

        private string RemoteAddress()
        {
            var conn = Context.Features.Get<IHttpContextFeature>()?.HttpContext?.Connection;
            return conn == null ? string.Empty : $"{conn.RemoteIpAddress}:{conn.RemotePort}";
        }

        public override async Task OnConnectedAsync()
        {
            _logger.Info($"hub:{Context.ConnectionId} Connected,{RemoteAddress()}");
            var code = AcbHttpContext.Current.GetProjectCode();
            if (string.IsNullOrWhiteSpace(code))
                return;
            Context.Items.Add(CodeKey, code);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.Info($"hub:{Context.ConnectionId} Disconnected,{RemoteAddress()}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
