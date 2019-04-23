using Acb.Core.EventBus;
using Acb.Core.Message;
using Acb.Core.Message.Codec;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Acb.RocketMq
{
    public static class ContainBuilderExtension
    {
        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="messageCodec"></param>
        /// <returns></returns>
        public static IServiceCollection UseRocketMqEventBus(this IServiceCollection services, RocketMqConfig config = null, IMessageCodec messageCodec = null)
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
                config = config ?? RocketMqConfig.Config();
                return new EventBusRocketMq(manager, codec, config);
            });
            return services;
        }
    }
}
