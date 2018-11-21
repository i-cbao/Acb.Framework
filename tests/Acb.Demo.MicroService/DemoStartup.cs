using Acb.Dapper;
using Acb.MicroService;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Demo.MicroService
{
    public class DemoStartup : MicroServiceStartup
    {
        protected override void MapServices(IServiceCollection services)
        {
            services.AddDapper(config =>
            {
                config.ConnectionString = "";
                config.ProviderName = "";
            });
            //services.AddRedisEventBus();
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            //provider.SubscribeAt();
            base.UseServices(provider);
        }
    }
}
