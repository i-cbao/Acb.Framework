using Acb.Backgrounder.Test.Tests;
using Acb.Framework;

namespace Acb.Backgrounder.Test
{
    internal class Program : ConsoleHost
    {
        private static void Main(string[] args)
        {
            //var tests = new CacheTest();
            //var tests = new EventBusTest();
            ConsoleTest tests;
            if (args.Length > 0 && args[0] == "client")
                tests = new TcpClientTest();
            else
                tests = new TcpSocketTest();

            Command += tests.OnCommand;
            MapServices += tests.OnMapServices;
            MapServiceCollection += tests.OnMapServiceCollection;

            UseServices += tests.OnUseServices;
            UseServiceProvider += tests.OnUseServiceProvider;
            StopEvent += tests.OnShutdown;
            Start(args);
        }
    }
}
