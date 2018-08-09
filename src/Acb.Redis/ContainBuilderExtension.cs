﻿using Acb.Core.EventBus;
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
        public static IServiceCollection UseRedisEventBus(this IServiceCollection services, string configName = null)
        {
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscriptionManager>();
                return new EventBusRedis(manager, configName);
            });
            return services;
        }

        /// <summary> 开启订阅 </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider SubscriptAt(this IServiceProvider provider)
        {
            provider.GetService<ISubscriptionAdapter>().SubscribeAt();
            return provider;
        }
    }
}
