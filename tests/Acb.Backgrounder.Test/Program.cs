using Acb.Backgrounder.Test.Jobs;
using Acb.Core.EventBus;
using Acb.Demo.Contracts.EventBus;
using Acb.Framework;
using Acb.Redis;
using Autofac;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Acb.Backgrounder.Test
{

    internal class Program : ConsoleHost
    {
        private static void Main(string[] args)
        {
            Command += OnCommand;
            MapServices += OnMapServices;
            UseServices += OnUseServices;
            Start(args);
        }

        private static void OnUseServices(IContainer provider)
        {
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
