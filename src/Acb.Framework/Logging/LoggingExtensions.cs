using Acb.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Acb.Framework.Logging
{
    public static class LoggingExtensions
    {
        /// <summary> 添加系统日志支持 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSystemLogging(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                var logLevel = LogManager.Level.Convert();
                if (logLevel != LogLevel.None)
                    builder.SetMinimumLevel(logLevel);
                builder.AddFilter((category, level) =>
                {
                    if ((category.StartsWith("Microsoft.") || category.StartsWith("System.")) &&
                        level < LogLevel.Warning)
                        return false;
                    return true;
                });
                builder.AddConsole();
            });
            LogManager.AddAdapter(new DefaultLoggerAdapter());
            return services;
        }
    }
}
