using System;
using Acb.Core.EventBus;
using Acb.MicroService;
using Acb.Redis;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.Demo.MicroService
{
    public class DemoStartup : MicroServiceStartup
    {
        protected override void MapServices(ContainerBuilder builder)
        {
            builder.RegisterType<EventBusRedis>().As<IEventBus>().SingleInstance();
            base.MapServices(builder);
        }

        protected override void UseServices(IServiceProvider provider)
        {
            provider.GetService<ISubscriptionAdapter>().SubscribeAt();
            base.UseServices(provider);
        }
    }
}
