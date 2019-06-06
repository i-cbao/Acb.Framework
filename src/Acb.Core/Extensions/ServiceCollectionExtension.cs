using Acb.Core.Logging;
using Acb.Core.Monitor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Acb.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary> 添加系统日志支持 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSystemLogging(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var adapter = new DefaultLoggerAdapter(provider);
                LogManager.AddAdapter(adapter);
                return adapter;
            });
            return services;
        }

        public static IServiceCollection AddMonitor(this IServiceCollection services, params Type[] monitorTypes)
        {
            if (monitorTypes != null && monitorTypes.Any())
            {
                foreach (var monitorType in monitorTypes)
                {
                    services.AddScoped(typeof(IMonitor), monitorType);
                }
            }

            services.AddSingleton<MonitorManager>();
            return services;
        }

        public static void Monitor(this IServiceProvider provider, MonitorData data)
        {
            var manager = provider.GetService<MonitorManager>();
            manager?.Record(data);
        }
    }
}
