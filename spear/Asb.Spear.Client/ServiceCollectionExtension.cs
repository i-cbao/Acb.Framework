using Acb.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asb.Spear.Client
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSpear(this IServiceCollection services, SpearOption option = null)
        {
            option = option ?? new SpearOption();
            services.TryAddSingleton(provider => new SpearClient(option));
            return services;
        }

        public static IServiceProvider UseSpear(this IServiceProvider provider, string[] modules = null, string jobName = null)
        {
            var client = provider.GetService<SpearClient>();
            Task.Factory.StartNew(async () =>
            {
                if (modules != null && modules.Any())
                {
                    await client.StartConfig(new ConfigOption
                    {
                        Mode = Consts.Mode.ToString().ToLower(),
                        ConfigModules = modules
                    });
                }
            });
            return provider;
        }
    }
}
