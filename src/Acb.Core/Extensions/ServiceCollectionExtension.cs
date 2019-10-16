using Acb.Core.Monitor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Acb.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static bool IsRegisted<T>(this IServiceCollection services)
        {
            return services.IsRegisted(typeof(T));
        }
        public static bool IsRegisted(this IServiceCollection services, Type serviceType)
        {
            return services.Any(t => t.ServiceType == serviceType);
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
