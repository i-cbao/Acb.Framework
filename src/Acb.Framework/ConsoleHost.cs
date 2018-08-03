using Acb.Core.Logging;
using Acb.Framework.Logging;
using Autofac;
using System;

namespace Acb.Framework
{
    public class ConsoleHost
    {
        protected static event Action<string, IContainer> Command;
        protected static event Action<ContainerBuilder> MapServices;
        protected static event Action<IContainer> UseServices;
        protected static DBootstrap Bootstrap { get; private set; }

        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static void Start(string[] args)
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            Bootstrap = new DBootstrap();
            Bootstrap.BuilderHandler += b => MapServices?.Invoke(b);
            Bootstrap.Initialize();
            UseServices?.Invoke(Bootstrap.Container);
            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "exit")
                    break;
                Command?.Invoke(cmd, Bootstrap.Container);
            }
        }
    }
}
