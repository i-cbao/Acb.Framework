using Acb.EventBus.RabbitMq;
using Autofac;
using System;

namespace Acb.EventBus.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            //builder.UseEventBusRedis(
            //    "140.143.96.228:6379,password=icb@888,abortConnect=false,connectRetry=3,connectTimeout=3000,defaultDatabase=0,syncTimeout=3000,version=3.2.1,responseTimeout=3000");
            //builder.UseEventBusRedis(new ConfigurationOptions
            //{
            //    EndPoints = { { "140.143.96.228", 6379 } },
            //    Password = "icb@888"
            //});
            builder.UseEventBusRabbitMq(new RabbitMqConfig
            {
                Host = "192.168.0.251",
                Port = 5672,
                UserName = "icb",
                Password = "a123456",
                Broker = "icb_broker",
                VirtualHost = "/icb"
            });
            var provider = builder.Build();
            provider.SubscribeAt();
            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "exit")
                    break;
                var bus = provider.Resolve<IEventBus>();
                bus.Publish(new UserEvent { Name = cmd });
            }
        }
    }
}
