using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Dependency;
using Acb.Core.Logging;
using Acb.Core.Reflection;
using Acb.Framework.Logging;
using Autofac;
using System;
using System.Linq;

namespace Acb.Framework
{
    /// <summary> 启动器 </summary>
    public class DBootstrap : Bootstrap
    {
        private bool _init;

        public ContainerBuilder Builder { get; private set; }

        private IContainer _container;

        /// <summary> Ioc容器 </summary>
        public IContainer Container => _container ?? (_container = Builder.Build());

        /// <summary> Ioc构建事件 </summary>
        public event Action<ContainerBuilder> BuilderHandler;

        internal void ReBuild(Action<ContainerBuilder> builderAction)
        {
            var updater = new ContainerBuilder();
            builderAction.Invoke(updater);
            updater.Update(_container);
        }

        /// <summary> 初始化 </summary>
        public override void Initialize()
        {
            if (_init) return;
            _init = true;

            LoggerInit();
            CacheInit();
            IocRegisters();
            BuilderHandler?.Invoke(Builder);
            _container = Builder.Build();
            DatabaseInit();
            ModulesInstaller();
        }

        /// <summary> Ioc注册 </summary>
        public override void IocRegisters()
        {
            Builder = new ContainerBuilder();
            //注入程序集查找器
            var finder = new DAssemblyFinder();
            Builder.RegisterInstance(finder).As<IAssemblyFinder>().SingleInstance();
            var assemblies = finder.FindAll().ToArray();
            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(IScopedDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired()//属性注入
                .InstancePerLifetimeScope(); //保证生命周期基于请求

            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired(); //属性注入

            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(ISingleDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired()//属性注入
                .SingleInstance(); //保证单例注入

            IocManager = new IocManager(this);
            Builder.RegisterInstance(IocManager).AsSelf().As<IIocManager>().SingleInstance();
        }

        public override void CacheInit()
        {
            CacheManager.SetProvider(CacheLevel.First, new RuntimeMemoryCacheProvider());
        }

        public override void LoggerInit()
        {
            LogManager.AddAdapter(new Log4NetAdapter());
        }

        public override void DatabaseInit()
        {
        }
    }
}
