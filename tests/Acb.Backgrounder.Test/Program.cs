using Acb.Backgrounder.Test.Jobs;
using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Demo.Contracts.EventBus;
using Acb.Framework;
using Acb.Redis;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using ILogger = Acb.Core.Logging.ILogger;

namespace Acb.Backgrounder.Test
{
    internal class Program : ConsoleHost
    {
        private static HubConnection _hubConnection;
        private static ILogger _logger;
        private static void Main(string[] args)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53454/config_hub",
                    opts =>
                    {
                        opts.Headers.Add("Authorization",
                            "acb 7SmYLfS0+/6pjdeTWZ8hW0ye1S30iVEjr/BuPkhXzdNXt3+LgRLr9YFQsXI7gobeFEK4YqxxdJhWhuUiBrbbwM7ETcIP1ANrL8xAsjxpcCtjdGYk32KYqme8oN9/n05h6rf05OZRBAoUq8z2fk5+84YucZ7tUJCORJRrN1IB4c6MWWz7SMOyHW31xfpVUo3vKAAKIyZHXRh8Ugaq7XmtNxPxhKgKfXwq");
                    })
                .Build();
            _logger = LogManager.Logger<Program>();
            Command += OnCommand;
            MapServices += OnMapServices;
            UseServices += OnUseServices;
            Start(args);
        }

        private static void OnUseServices(IContainer provider)
        {
            _hubConnection.On<object>("UPDATE", config =>
            {
                _logger.Info(config);
            });
            try
            {
                _hubConnection.StartAsync().Wait();
                _hubConnection.SendAsync("Subscript", new[] { "basic" }, "dev");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            //开启订阅
            provider.Resolve<ISubscriptionAdapter>().SubscribeAt();
            StartScheduler().GetAwaiter().GetResult();
        }

        private static void OnMapServices(ContainerBuilder builder)
        {
            //注册事件总线
            //builder.RegisterType<EventBusRabbitMq>().As<IEventBus>().SingleInstance();
            builder.RegisterType<EventBusRedis>().As<IEventBus>().SingleInstance();
        }

        private static void OnCommand(string cmd, IContainer provider)
        {
            var bus = provider.Resolve<IEventBus>();
            bus.Publish(new UserEvent { Name = cmd });
            bus.Publish(new TestEvent { Content = cmd });
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
