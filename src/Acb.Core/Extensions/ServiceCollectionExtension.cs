using Acb.Core.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary> 添加系统日志支持 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSystemLogging(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var adapter = new DefaultLoggerAdapter(provider);
                LogManager.AddAdapter(adapter);
                return adapter;
            });
            return services;
        }
    }
}
