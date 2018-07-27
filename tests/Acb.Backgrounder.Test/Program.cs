using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Demo.Contracts.EventBus;
using Acb.Framework;
using Acb.Framework.Logging;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Acb.Backgrounder.Test.Jobs;
using Quartz;
using Quartz.Impl;

namespace Acb.Backgrounder.Test
{

    internal class Program
    {
        private static void Main(string[] args)
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            var bootstrap = new DBootstrap();
            bootstrap.Initialize();

            StartScheduler().GetAwaiter().GetResult();

            //发布
            string input;
            var bus = CurrentIocManager.IocManager.Resolve<IEventBus>();
            do
            {
                input = Console.ReadLine();
                bus.Publish(new UserEvent { Name = input });
            } while (!string.IsNullOrWhiteSpace(input) && input != "exit");
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
