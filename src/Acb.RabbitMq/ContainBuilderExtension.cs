using Acb.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.RabbitMq
{
    public static class ContainBuilderExtension
    {
        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, string configName = null)
        {
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscriptionManager>();
                var connection = new DefaultRabbitMqConnection(configName);
                return new EventBusRabbitMq(connection, manager);
            });
            return services;
        }

        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, RabbitMqConfig config = null)
        {
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscriptionManager>();
                var connection = new DefaultRabbitMqConnection(config);
                return new EventBusRabbitMq(connection, manager);
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
