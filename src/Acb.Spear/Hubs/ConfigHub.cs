using Acb.Core;
using Acb.Spear.Domain;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Hubs
{
    /// <summary> 配置中心总线 </summary>
    public class ConfigHub : Hub
    {
        private const string CodeKey = "code";
        /// <summary> 订阅配置 </summary>
        /// <param name="modes">模块</param>
        /// <param name="env">环境模式</param>
        /// <returns></returns>
        public async Task Subscript(string[] modes, string env)
        {
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
            Context.Items.TryGetValue(CodeKey, out var code);
            if (code == null || string.IsNullOrWhiteSpace(code.ToString()))
                return;
            foreach (var mode in modes)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{code}_{mode}_{env}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            var code = AcbHttpContext.Current.Request.GetProjectCode();
            if (string.IsNullOrWhiteSpace(code))
                return;
            Context.Items.Add(CodeKey, code);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
