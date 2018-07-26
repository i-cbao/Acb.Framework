using Acb.Core.Logging;
using Acb.Framework;
using Acb.Framework.Logging;

namespace Acb.EventBus.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            var bootstrap = new DBootstrap();
            bootstrap.Initialize();
        }
    }
}
