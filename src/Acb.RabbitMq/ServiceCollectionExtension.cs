using Acb.Core.EventBus;
using Acb.Core.Message;
using Acb.Core.Message.Codec;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.RabbitMq
{
    public static class ServiceCollectionExtension
    {
        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <param name="messageCodec"></param>
        /// <param name="retryTimes"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, string configName = null,
            IMessageCodec messageCodec = null, TimeSpan[] retryTimes = null)
        {
            if (messageCodec != null)
            {
                services.TryAddSingleton(messageCodec);
            }
            else
            {
                services.TryAddSingleton<IMessageCodec, JsonMessageCodec>();
            }

            services.AddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscribeManager>();
                var codec = provider.GetService<IMessageCodec>();
                var connection = new DefaultRabbitMqConnection(configName);
                return new EventBusRabbitMq(connection, manager, codec, retryTimes);
            });
            return services;
        }

        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="retryTimes"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, RabbitMqConfig config, TimeSpan[] retryTimes = null)
        {
            services.TryAddSingleton<IMessageCodec, JsonMessageCodec>();
            services.TryAddSingleton<IEventBus>(provider =>
            {
                var manager = provider.GetService<ISubscribeManager>();
                var codec = provider.GetService<IMessageCodec>();
                var connection = new DefaultRabbitMqConnection(config);
                return new EventBusRabbitMq(connection, manager, codec, retryTimes);
            });
            return services;
        }
    }
}
