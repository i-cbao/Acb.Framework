using Acb.Core.EventBus;
using Acb.Framework;
using Acb.RabbitMq;
using Acb.Redis;
using Autofac;

namespace Acb.EventBus.Test
{
    internal class Program : ConsoleHost
    {
        private static void OnMapServices(ContainerBuilder builder)
        {
            builder.RegisterType<EventBusRabbitMq>().As<IEventBus>().SingleInstance();
        }

        private static void OnUseServices(IContainer provider)
        {
            provider.Resolve<ISubscriptionAdapter>().SubscribeAt();
        }

        private static void OnCommand(string command, IContainer provider)
        {
            var bus = provider.Resolve<IEventBus>();
            bus.Publish(new UserEvent { Name = command });
        }

        private static void Main(string[] args)
        {
            MapServices += OnMapServices;
            UseServices += OnUseServices;
            Command += OnCommand;
            Start(args);
        }

    }
}
