using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Asb.Spear.Client
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSpear(this IServiceCollection services, Action<SpearOption> optionAction = null)
        {
            var option = new SpearOption();
            optionAction?.Invoke(option);
            services.TryAddSingleton(provider =>
            {
                var conn = new HubConnectionBuilder()
                    .WithUrl(option.HubAddress, opts =>
                    {
                        opts.Headers.Add("Authorization",
                            "acb EQS9LTGKzNOHiCn0+8avXJIDCiLW/KtraWRAnl1874nNBAcZ0nPd8KZXUXLC+OnCevPWKVQzju/ZLcSExoq+ps3pwpBGpKtK0ZMOfQoPsu4uvhyRvbuU66eaYaH6w1sPMDLpmxHwBi3C8Mc3bdk4Bi1EC8SYlPct22K+gLG6vAM=");
                    })
                    .Build();
                return conn;
            });
            return services;
        }
    }
}
