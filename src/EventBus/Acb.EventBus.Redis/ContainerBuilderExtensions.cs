using Autofac;
using StackExchange.Redis;

namespace Acb.EventBus.Redis
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder UseEventBusRedis(this ContainerBuilder builder, string connectionString)
        {
            builder.RegisterType<EventBusRedis>().As<IEventBus>().SingleInstance();
            builder.Register(provider => new RedisConnection(connectionString)).As<IRedisConnection>();
            builder.UseEventBus();
            return builder;
        }

        public static ContainerBuilder UseEventBusRedis(this ContainerBuilder builder, ConfigurationOptions opts)
        {
            builder.RegisterType<EventBusRedis>().As<IEventBus>().SingleInstance();
            builder.Register(provider => new RedisConnection(opts)).As<IRedisConnection>();
            builder.UseEventBus();
            return builder;
        }
    }
}
