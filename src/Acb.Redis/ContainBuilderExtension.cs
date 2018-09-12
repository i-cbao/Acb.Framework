using Acb.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.Redis
{
    public static class ContainBuilderExtension
    {
        /// <summary> 使用Redis事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisEventBus(this IServiceCollection services, string configName = null)
        {
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscriptionManager>();
                return new EventBusRedis(manager, configName);
            });
            return services;
        }
    }
}
