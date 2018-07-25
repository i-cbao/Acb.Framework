using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Demo.Contracts.EventBus;
using Acb.Framework;
using System;
using System.Threading.Tasks;
using Acb.Framework.Logging;

namespace Acb.Backgrounder.Test
{

    public class MessageHandler : IIntegrationEventHandler<TestEvent>
    {
        public Task Handle(TestEvent @event)
        {
            Console.WriteLine("one:" + JsonHelper.ToJson(@event));
            return Task.CompletedTask;
        }
    }

    public class MessageHandlerTwo : IIntegrationEventHandler<TestEvent>
    {
        public Task Handle(TestEvent @event)
        {
            Console.WriteLine("two:" + JsonHelper.ToJson(@event));
            return Task.CompletedTask;
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            var bootstrap = new DBootstrap();
            bootstrap.Initialize();
            string input;
            var bus = CurrentIocManager.IocManager.Resolve<IEventBus>();
            do
            {
                input = Console.ReadLine();
                bus.Publish(new TestEvent { Content = input });
            } while (!string.IsNullOrWhiteSpace(input) && input != "exit");

            //const string queue = "shay";
            //var factory = new ConnectionFactory { HostName = "192.168.0.252" };
            //using (var conn = factory.CreateConnection())
            //{
            //    using (var channel = conn.CreateModel())
            //    {
            //        channel.QueueDeclare(queue: queue,
            //            durable: false,
            //            exclusive: false,
            //            autoDelete: false,
            //            arguments: null);

            //        var consumer = new EventingBasicConsumer(channel);
            //        consumer.Received += Consumer_Received;
            //        channel.BasicConsume(queue: queue,
            //            autoAck: false,//自动确认
            //            consumer: consumer);

            //        Console.WriteLine(" Press [enter] to exit.");
            //        Console.ReadLine();
            //    }
            //}
            //new DBootstrap().Initialize();
            //LogManager.ClearAdapter();
            //LogManager.AddAdapter(new ConsoleAdapter(), LogLevel.All);
            //LogManager.LogLevel(LogLevel.All);
            //var host = new ConsoleHost(new IJob[]
            //{
            //    new TestJob("hello_5s", TimeSpan.FromSeconds(5)),
            //    new TestJob("hello_10s", TimeSpan.FromSeconds(10))
            //});
            //host.OnCommand += HostCommand;
            //var logger = LogManager.Logger<Program>();

            ////host.Manager.OnLog += logger.Debug;
            //host.Manager.OnException += e => logger.Error(e.Message, e);
            //host.Start();
        }

        private static bool HostCommand(string arg)
        {
            LogManager.Logger<Program>().Info(arg);
            return false;
        }
    }
}
