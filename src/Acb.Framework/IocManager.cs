using Acb.Core.Dependency;
using Autofac;
using System;

namespace Acb.Framework
{
    public class IocManager : IIocManager
    {
        private readonly DBootstrap _bootstrap;

        private IContainer Current => _bootstrap.Container;

        public IocManager(DBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
        }

        public void MapService(Action<ContainerBuilder> buidlerAction)
        {
            _bootstrap.ReBuild(buidlerAction);
        }

        /// <summary> 获取Ioc注入实例 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            if (Current.TryResolve<T>(out var instance))
                return instance;
            throw new Exception($"ioc注入异常,Type:{typeof(T).FullName}");
        }

        /// <summary> 获取Ioc注入实例 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            if (Current.TryResolve(type, out var instance))
                return instance;
            throw new Exception($"ioc注入异常,Type:{type.FullName}");
        }

        public T Resolve<T>(string key)
        {
            return Current.ResolveKeyed<T>(key);
        }

        public object Resolve(string key, Type type)
        {
            return Current.ResolveKeyed(key, type);
        }

        /// <summary> 是否注册Ioc注入 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type)
        {
            return Current.IsRegistered(type);
        }

        public bool IsRegistered(string key, Type type)
        {
            return Current.IsRegisteredWithKey(key, type);
        }

        public bool IsRegistered<T>(string key)
        {
            return Current.IsRegisteredWithKey<T>(key);
        }

        public bool IsRegistered<T>()
        {
            return Current.IsRegistered<T>();
        }
    }
}
