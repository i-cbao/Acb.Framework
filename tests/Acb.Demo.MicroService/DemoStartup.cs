using Acb.MicroService;
using Acb.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Demo.MicroService
{
    public class DemoStartup : MicroServiceStartup
    {
        protected override void MapServices(IServiceCollection services)
        {
            services.AddRedisEventBus();
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            provider.SubscriptAt();
            base.UseServices(provider);
        }
    }
}
