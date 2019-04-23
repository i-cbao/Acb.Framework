using Acb.Backgrounder.Test.Jobs;
using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Demo.Contracts.EventBus;
using Acb.Framework;
using Acb.RabbitMq;
using Acb.RabbitMq.Options;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using Acb.Backgrounder.Test.EventBus;
using ILogger = Acb.Core.Logging.ILogger;

namespace Acb.Backgrounder.Test
{
    internal class Program : ConsoleHost
    {
        //private static HubConnection _hubConnection;
        private static ILogger _logger;
        private static void Main(string[] args)
        {
            //const string url = "http://localhost:53454/config_hub";
            //const string url = "http://192.168.0.252:6306/config_hub";
            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl(url, opts =>
            //    {
            //        opts.Headers.Add("Authorization",
            //            "acb EQS9LTGKzNOHiCn0+8avXJIDCiLW/KtraWRAnl1874nNBAcZ0nPd8KZXUXLC+OnCevPWKVQzju/ZLcSExoq+ps3pwpBGpKtK0ZMOfQoPsu4uvhyRvbuU66eaYaH6w1sPMDLpmxHwBi3C8Mc3bdk4Bi1EC8SYlPct22K+gLG6vAM=");
            //    })
            //    .Build();
            //var attr = typeof(MessageHandler).GetCustomAttribute<SubscriptionAttribute>();

            _logger = LogManager.Logger<Program>();
            Command += OnCommand;
            MapServices += OnMapServices;
            MapServiceCollection += OnMapServiceCollection;

            UseServices += OnUseServices;
            UseServiceProvider += OnUseServiceProvider;
            Start(args);
        }

        private static void OnUseServiceProvider(IServiceProvider provider)
        {
            //开启订阅
            provider.SubscribeAt();
        }

        private static void OnUseServices(IContainer provider)
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

        private static void OnMapServiceCollection(IServiceCollection services)
        {
            //注册事件总线
            //RabbitMQ
            services.AddRabbitMqEventBus();
            //Redis
            //services.AddRedisEventBus();
        }

        private static void OnMapServices(ContainerBuilder builder)
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

        private static void OnCommand(string cmd, IContainer provider)
        {
            var bus = provider.Resolve<IEventBus>();
            bus.Publish(new UserEvent { Name = cmd }, new RabbitMqPublishOption
            {
                Delay = TimeSpan.FromSeconds(2)
            });
            //bus.Publish("icb_framework_simple_queue", cmd, 2 * 1000);
            _logger.Info($"Send Message:{cmd}");
            bus.Publish(new TestEvent { Content = cmd }, new RabbitMqPublishOption
            {
                Delay = TimeSpan.FromMinutes(5)
            });
            //var queue = provider.Resolve<IMessageQueue>();
            //queue.Send(cmd);
        }

        private static async Task StartScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);
            var scheduler = await factory.GetScheduler();
            var job = JobBuilder.Create<TestJob>().WithIdentity("test_job").Build();
            var trigger = TriggerBuilder.Create()
                .WithSimpleSchedule(b => b.WithIntervalInSeconds(10).RepeatForever())
                .Build();
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
    }
}
