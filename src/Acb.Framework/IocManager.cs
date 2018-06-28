using Autofac;
using Acb.Core.Dependency;
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
        public T Resolve<T>()
        {
            _bootstrap.Container.TryResolve<T>(out var instance);
            return instance;
        }

        public object Resolve(Type type)
        {
            _bootstrap.Container.TryResolve(type, out var instance);
            return instance;
        }

        public bool IsRegistered(Type type)
        {
            return _bootstrap.Container.IsRegistered(type);
        }
    }
}
