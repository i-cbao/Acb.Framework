using Acb.Backgrounder.Test.Tests;
using Acb.Framework;

namespace Acb.Backgrounder.Test
{
    internal class Program : ConsoleHost
    {
        private static void Main(string[] args)
        {
            //var tests = new CacheTest();
            var tests = new EventBusTest();

            Command += tests.OnCommand;
            MapServices += tests.OnMapServices;
            MapServiceCollection += tests.OnMapServiceCollection;

            UseServices += tests.OnUseServices;
            UseServiceProvider += tests.OnUseServiceProvider;
            Start(args);
        }
    }
}
