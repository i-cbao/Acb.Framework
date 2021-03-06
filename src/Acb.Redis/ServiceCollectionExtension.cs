﻿using Acb.Core.Cache;
using Acb.Core.EventBus;
using Acb.Core.Message;
using Acb.Core.Message.Codec;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.Redis
{
    public static class ServiceCollectionExtension
    {
        /// <summary> 使用Redis事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <param name="messageCodec"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisEventBus(this IServiceCollection services, string configName = null, IMessageCodec messageCodec = null)
        {
            if (messageCodec != null)
            {
                services.TryAddSingleton(messageCodec);
            }
            else
            {
                services.TryAddSingleton<IMessageCodec, JsonMessageCodec>();
            }
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscribeManager>();
                var codec = provider.GetService<IMessageCodec>();
                return new EventBusRedis(manager, codec, configName);
            });
            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services,
            Action<RedisConfig> configAction = null, string region = null)
        {
            services.TryAddSingleton<ICache>(provider =>
            {
                var config = RedisConfig.Config();
                configAction?.Invoke(config);
                return new RedisCache(region, config);
            });
            services.TryAddTransient(provider =>
            {
                var config = RedisConfig.Config();
                configAction?.Invoke(config);
                return new RedisCache(region, config);
            });
            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services,
            string configName, string region = null)
        {
            services.TryAddSingleton<ICache>(provider =>
            {
                var config = RedisConfig.Config(configName);
                return new RedisCache(region, config);
            });
            services.TryAddSingleton(provider =>
            {
                var config = RedisConfig.Config(configName);
                return new RedisCache(region, config);
            });
            return services;
        }

        /// <summary> 开启两级缓存同步 </summary>
        /// <param name="provider"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static IServiceProvider UseCacheSync(this IServiceProvider provider, string configName = null)
        {
            //订阅
            RedisManager.Instance.CacheSubscribe(configName);
            return provider;
        }
    }
}
