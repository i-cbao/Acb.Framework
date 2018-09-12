using Acb.Core.Logging;
using Acb.Framework.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Framework
{
    /// <summary> 控制台宿主机 </summary>
    public class ConsoleHost
    {
        /// <summary> 命令事件 </summary>
        protected static event Action<string, IContainer> Command;
        /// <summary> 注册服务 </summary>
        protected static event Action<IServiceCollection> MapServiceCollection;
        /// <summary> 注册服务 </summary>
        protected static event Action<ContainerBuilder> MapServices;

        /// <summary> 使用服务 </summary>
        protected static event Action<IServiceProvider> UseServiceProvider;
        /// <summary> 使用服务 </summary>
        protected static event Action<IContainer> UseServices;

        /// <summary> 启动项 </summary>
        protected static DBootstrap Bootstrap { get; private set; }

        /// <summary> 开启服务 </summary>
        /// <param name="args"></param>
        public static void Start(string[] args)
        {
            LogManager.AddAdapter(new ConsoleAdapter());
            Bootstrap = new DBootstrap();
            var services = new ServiceCollection();
            MapServiceCollection?.Invoke(services);
            Bootstrap.BuilderHandler += b =>
            {
                b.Populate(services);
                MapServices?.Invoke(b);
            };
            Bootstrap.Initialize();
            if (UseServiceProvider != null)
            {
                var provider = new AutofacServiceProvider(Bootstrap.Container);
                UseServiceProvider.Invoke(provider);
            }

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
