using Acb.Core.Dependency;
using Acb.Spear.Contracts;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Hubs
{
    /// <summary> 配置中心总线 </summary>
    public class ConfigHub : SpearHub
    {
        /// <summary> 订阅配置 </summary>
        /// <param name="modules">模块</param>
        /// <param name="env">环境模式</param>
        /// <returns></returns>
        public async Task Subscript(string[] modules, string env)
        {
            if (string.IsNullOrWhiteSpace(Code))
                return;
            Logger.Info($"hub:{Context.ConnectionId} Subscript {env} - {string.Join(',', modules)}");
            foreach (var mode in modules)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"{Code}_{mode}_{env}");
            }

            if (ProjectId.HasValue)
            {
                var contract = CurrentIocManager.Resolve<IConfigContract>();
                var dict = new Dictionary<string, object>();
                foreach (var module in modules)
                {
                    var config = await contract.GetAsync(ProjectId.Value, module, env);
                    dict.Add(module, config);
                }
                await Clients.Caller.SendAsync("UPDATE", dict);
            }
        }

        /// <summary> 取消订阅配置 </summary>
        /// <param name="modes">模块</param>
        /// <param name="env">环境模式</param>
        /// <returns></returns>
        public async Task UnSubscript(string[] modes, string env)
        {
            Logger.Info($"hub:{Context.ConnectionId} UnSubscript {env} - {string.Join(',', modes)}");
            if (string.IsNullOrWhiteSpace(Code))
                return;
            foreach (var mode in modes)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{Code}_{mode}_{env}");
            }
        }
    }

    /// <summary> 配置hub扩展 </summary>
    public static class ConfigHubExtensions
    {
        /// <summary> 更新配置通知 </summary>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <param name="module"></param>
        /// <param name="mode"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task UpdateAsync(this IHubContext<ConfigHub> context, string code, string module,
            string mode, object config)
        {
            await context.Clients.Group($"{code}_{module}_{mode}")
                .SendAsync("UPDATE", new Dictionary<string, object>
                {
                    {module, config}
                });
        }
    }
}
