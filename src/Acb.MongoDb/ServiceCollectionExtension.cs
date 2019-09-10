using Acb.Core.Config;
using Acb.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.MongoDb
{
    public static class ContainBuilderExtension
    {
        /// <summary> 使用MongoDB </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <param name="database"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoDb(this IServiceCollection services, string configName = null,
            string database = null, string prefix = null)
        {
            services.AddMongoDb(config =>
            {
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    var item = MongoConfig.Config(configName);
                    if (item != null)
                    {
                        config = item;
                    }
                }
                if (!string.IsNullOrWhiteSpace(database))
                    config.Database = database;
            }, prefix);
            return services;
        }

        /// <summary> 使用MongoDB </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoDb(this IServiceCollection services, Action<MongoConfig> configAction, string prefix = null)
        {
            services.TryAddSingleton(provider =>
            {
                var config = MongoConfig.Config();
                configAction?.Invoke(config);
                if (string.IsNullOrWhiteSpace(config.Database))
                {
                    config.Database = "mongo_db".Const<string>();
                }
                return new MongoHelper(config, prefix);
            });

            services.TryAddSingleton(provider =>
            {
                var helper = provider.GetService<MongoHelper>();
                return helper.Database;
            });
            return services;
        }
    }
}
