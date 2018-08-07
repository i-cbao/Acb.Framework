using Autofac;

namespace Acb.EventBus.RabbitMq
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder UseEventBusRabbitMq(this ContainerBuilder builder, RabbitMqConfig config)
        {
            builder.RegisterType<EventBusRabbitMq>().As<IEventBus>().SingleInstance();
            builder.Register(provider => new DefaultRabbitMqConnection(config)).As<IRabbitMqConnection>();
            builder.UseEventBus();
            return builder;
        }
    }
}
