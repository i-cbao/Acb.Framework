using Acb.Core.Extensions;
using Acb.Core.Monitor;
using Acb.Dapper;
using Acb.MicroService.Host;
using Acb.Middleware.Monitor;
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

            //services.AddMessagePackCodec();
            //services.AddRedisEventBus();
            services.AddMonitor(new[] { typeof(LoggerMonitor), typeof(AcbMonitor) });
            base.MapServices(services);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            //provider.SubscribeAt();
            base.UseServices(provider);
        }
    }
}
