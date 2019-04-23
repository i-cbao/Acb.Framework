using Acb.Dapper;
using Acb.MicroService.Host;
using Microsoft.Extensions.DependencyInjection;
using System;
using Acb.Core.EventBus;

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

            //services.AddMessagePackCodec();
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
