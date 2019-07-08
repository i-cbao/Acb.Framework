using Acb.Core.EventBus;
using Acb.Demo.Contracts.EventBus;
using Acb.RabbitMq;
using Acb.RabbitMq.Options;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Backgrounder.Test.Tests
{
    public class EventBusTest : ConsoleTest
    {
        public override void OnUseServiceProvider(IServiceProvider provider)
        {
            //开启订阅
            provider.SubscribeAt();
        }
        public override void OnMapServiceCollection(IServiceCollection services)
        {
            //注册事件总线
            //RabbitMQ
            services.AddRabbitMqEventBus("spartner");
            services.AddRabbitMqEventBus();
            //Redis
            //services.AddRedisEventBus();
        }

        public override void OnUseServices(IContainer provider)
        {
            //_hubConnection.On<object>("UPDATE", config =>
            //{
            //    _logger.Info(config);
            //});
            //try
            //{
            //    _hubConnection.StartAsync().Wait();
            //    _hubConnection.SendAsync("Subscript", new[] { "basic" }, "dev");
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error(ex.Message, ex);
            //}

            //gps
            //var queue = provider.Resolve<IMessageQueue>();
            ////var list = queue.Receive<string>(2).Result;
            ////_logger.Info(list);
            //queue.Subscibe<string>(t =>
            //{
            //    _logger.Info(t);
            //    //throw new Exception("dds");
            //});
        }

        public override void OnMapServices(ContainerBuilder builder)
        {
            builder.Register(provider =>
            {
                var conn = new DefaultRabbitMqConnection("gps");
                return new RabbitMessageQueue(conn, "testDcpProductId");
            }).As<IMessageQueue>().SingleInstance();

            //RocketMQ
            //builder.Register(provider =>
            //{
            //    var manager = provider.Resolve<ISubscriptionManager>();
            //    //var config = new RocketMqConfig
            //    //{
            //    //    Host = "220.167.101.49:9876",
            //    //    Topic = "icb_topic",
            //    //    ProducerId = "icb_producer",
            //    //    ConsumerId = "icb_consumer"
            //    //};
            //    return new EventBusRocketMq(manager, RocketMqConfig.Config());
            //}).As<IEventBus>().SingleInstance();
        }

        public override void OnCommand(string cmd, IContainer provider)
        {
            var bus = provider.Resolve<IEventBus>();
            bus.Publish(new UserEvent { Name = cmd }, new RabbitMqPublishOption
            {
                Delay = TimeSpan.FromSeconds(2)
            });
            //bus.Publish("icb_framework_simple_queue", cmd, 2 * 1000);
            Logger.Info($"Send Message:{cmd}");
            var sbus = provider.Resolve<IServiceProvider>().GetEventBus("spartner");
            sbus.Publish(new TestEvent { Content = cmd }, new RabbitMqPublishOption
            {
                Delay = TimeSpan.FromSeconds(10)
            });
            //var queue = provider.Resolve<IMessageQueue>();
            //queue.Send(cmd);
        }
    }
}
