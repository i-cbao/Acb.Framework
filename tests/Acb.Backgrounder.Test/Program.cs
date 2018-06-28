using Acb.Backgrounder.Test.Jobs;
using Acb.Core.Logging;
using Acb.Framework;
using Acb.Framework.Logging;
using System;

namespace Acb.Backgrounder.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new DBootstrap().Initialize();
            LogManager.ClearAdapter();
            LogManager.AddAdapter(new ConsoleAdapter(), LogLevel.All);
            LogManager.LogLevel(LogLevel.All);
            var host = new ConsoleHost(new IJob[]
            {
                new TestJob("hello_5s", TimeSpan.FromSeconds(5)),
                new TestJob("hello_10s", TimeSpan.FromSeconds(10))
            });
            host.OnCommand += HostCommand;
            var logger = LogManager.Logger<Program>();

            //host.Manager.OnLog += logger.Debug;
            host.Manager.OnException += e => logger.Error(e.Message, e);
            host.Start();
        }

        private static bool HostCommand(string arg)
        {
            LogManager.Logger<Program>().Info(arg);
            return false;
        }
    }
}
