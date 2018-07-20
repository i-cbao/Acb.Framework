using Acb.Core.Dependency;
using Autofac;
using System;

namespace Acb.Framework
{
    public class IocManager : IIocManager
    {
        private readonly DBootstrap _bootstrap;

        public IocManager(DBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
        }

        /// <summary> 获取Ioc注入实例 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            if (_bootstrap.Container.TryResolve<T>(out var instance))
                return instance;
            throw new Exception($"ioc注入异常,Type:{typeof(T).FullName}");
        }

        /// <summary> 获取Ioc注入实例 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            if (_bootstrap.Container.TryResolve(type, out var instance))
                return instance;
            throw new Exception($"ioc注入异常,Type:{type.FullName}");
        }

        /// <summary> 是否注册Ioc注入 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type)
        {
            return _bootstrap.Container.IsRegistered(type);
        }
    }
}
