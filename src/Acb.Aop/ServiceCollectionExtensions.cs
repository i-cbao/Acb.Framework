using Acb.Aop.Impl;
using Acb.ProxyGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.Aop
{
    public static class ServiceCollectionExtensions
    {
        /// <summary> 添加AOP拦截 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAop(this IServiceCollection services)
        {
            services.AddProxy();
            services.AddSingleton<IAopFactory, AopFactory>();
            return services;
        }
    }
}
