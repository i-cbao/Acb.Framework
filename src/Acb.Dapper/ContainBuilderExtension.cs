using Acb.Core.Data.Config;
using Acb.Core.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.Dapper
{
    public static class ContainBuilderExtension
    {
        public static IServiceCollection AddDapper(this IServiceCollection services,
            Action<ConnectionConfig> configAction = null)
        {
            services.TryAddTransient<IUnitOfWork>(provider =>
            {
                var config = ConnectionConfig.Config();
                configAction?.Invoke(config);
                return new UnitOfWork(config.ConnectionString, config.ProviderName);
            });
            return services;
        }

        public static IServiceCollection AddDapper(this IServiceCollection services, string configName)
        {
            services.TryAddScoped<IUnitOfWork>(provider =>
            {
                var config = ConnectionConfig.Config(configName);
                return new UnitOfWork(config.ConnectionString, config.ProviderName);
            });
            return services;
        }
    }
}
