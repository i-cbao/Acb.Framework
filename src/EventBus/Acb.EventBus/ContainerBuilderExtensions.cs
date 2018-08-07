using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Acb.EventBus
{
    public static class ContainerBuilderExtensions
    {
        private static List<Assembly> FindAssemblies()
        {
            var asses = new List<Assembly>();
            var dps = DependencyContext.Default;
            var libs = dps.CompileLibraries;

            foreach (var lib in libs)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    asses.Add(assembly);
                }
                catch
                {
                }
            }

            return asses;
        }

        /// <summary> 基础事件总线 </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder UseEventBus(this ContainerBuilder builder)
        {
            builder.RegisterType<DefaultSubscriptionManager>().As<ISubscriptionManager>().SingleInstance();
            builder.RegisterType<DefaultConsumeConfigurator>().As<IConsumeConfigurator>().SingleInstance();
            builder.RegisterType<DefaultSubscriptionAdapter>().As<ISubscriptionAdapter>().SingleInstance();
            var asses = FindAssemblies().ToArray();
            builder.RegisterAssemblyTypes(asses)
                .Where(t => typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(t))
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterAssemblyTypes(asses)
                .Where(t => typeof(IEventHandler).IsAssignableFrom(t)).SingleInstance();
            return builder;
        }

        /// <summary> 启动订阅 </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IContainer SubscribeAt(this IContainer provider)
        {
            provider.Resolve<IConsumeConfigurator>().Provider = provider;
            provider.Resolve<ISubscriptionAdapter>().SubscribeAt();
            return provider;
        }
    }
}
