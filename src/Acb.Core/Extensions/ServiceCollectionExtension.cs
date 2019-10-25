using Acb.Core.Monitor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Acb.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        private static Func<MonitorData, bool> _monitorFilter;

        public static bool IsRegisted<T>(this IServiceCollection services)
        {
            return services.IsRegisted(typeof(T));
        }
        public static bool IsRegisted(this IServiceCollection services, Type serviceType)
        {
            return services.Any(t => t.ServiceType == serviceType);
        }

        public static IServiceCollection AddMonitor(this IServiceCollection services, Type[] types,
            Func<MonitorData, bool> filter = null)
        {
            _monitorFilter = filter;
            if (types != null && types.Any())
            {
                foreach (var monitorType in types)
                {
                    services.AddScoped(typeof(IMonitor), monitorType);
                }
            }

            services.AddSingleton<MonitorManager>();
            return services;
        }

        public static void Monitor(this IServiceProvider provider, MonitorData data)
        {
            if (_monitorFilter == null || !_monitorFilter(data))
                return;
            var manager = provider.GetService<MonitorManager>();
            manager?.Record(data);
        }
    }
}
