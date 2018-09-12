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
                config = config ?? RocketMqConfig.Config();
                return new EventBusRocketMq(manager, config);
            });
            return services;
        }
    }
}
