using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Acb.MongoDb
{
    public static class ContainBuilderExtension
    {
        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="configName"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoDb(this IServiceCollection services, string configName = null, string prefix = null)
        {
            var config = MongoConfig.Config(configName);
            services.AddMongoDb(config, prefix);
            return services;
        }
        /// <summary> 使用RabbitMQ事件总线 </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoDb(this IServiceCollection services, MongoConfig config = null, string prefix = null)
        {
            services.TryAddSingleton(provider =>
            {
                config = config ?? MongoConfig.Config();
                return new MongoHelper(config, prefix);
            });
            return services;
        }
    }
}
