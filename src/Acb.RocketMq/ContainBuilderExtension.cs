using System;
using Acb.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Acb.RocketMq
{
    public static class ContainBuilderExtension
    {
        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection UseRocketMqEventBus(this IServiceCollection services, RocketMqConfig config = null)
        {
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscriptionManager>();
                var connection = new DefaultRocketMqConnection(config);
                return new EventBusRocketMq(manager, connection);
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
