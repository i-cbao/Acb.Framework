using Acb.MicroService.PureClient.Codec;
using Acb.MicroService.PureClient.Router;
using Acb.MicroService.PureClient.Router.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Acb.MicroService.PureClient
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 添加微服务客户端 </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMicroClient(this IServiceCollection services, Action<MicroServiceConfig> configAction = null)
        {
            services.AddHttpClient();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IMessageCodec, JsonMessageCodec>();
            services.TryAddSingleton<IRouter>(provider =>
            {
                var config = new MicroServiceConfig();
                configAction?.Invoke(config);
                var router = provider.GetService<ILogger<ConsulRouter>>();
                return new ConsulRouter(config, router);
            });
            return services;
        }

        /// <summary> 使用微服务客户端 </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider UseMicroClient(this IServiceProvider provider)
        {
            Extensions.CurrentProvider = provider;
            return provider;
        }

        /// <summary> 使用微服务客户端 </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMicroClient(this IApplicationBuilder builder)
        {
            Extensions.CurrentProvider = builder.ApplicationServices;
            return builder;
        }

        /// <summary> 添加服务代理 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddProxy<T>(this IServiceCollection services) where T : class
        {
            services.TryAddSingleton(provider => ProxyService.Proxy<T>());
            return services;
        }

        /// <summary> 服务代理 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T Proxy<T>(this IServiceProvider provider) where T : class
        {
            return ProxyService.Proxy<T>();
        }
    }
}
