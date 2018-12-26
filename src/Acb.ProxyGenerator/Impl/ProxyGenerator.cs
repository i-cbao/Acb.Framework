using Acb.ProxyGenerator.Activator;
using System;

namespace Acb.ProxyGenerator.Impl
{
    public sealed class ProxyGenerator : IProxyGenerator
    {
        private readonly IProxyTypeGenerator _proxyTypeGenerator;
        private readonly IProxyActivatorFactory _activatorFactory;

        public ProxyGenerator(IProxyTypeGenerator proxyTypeGenerator, IProxyActivatorFactory activatorFactory)
        {
            _proxyTypeGenerator = proxyTypeGenerator ?? throw new ArgumentNullException(nameof(proxyTypeGenerator));
            _activatorFactory = activatorFactory ?? throw new ArgumentNullException(nameof(activatorFactory));
        }

        public IProxyTypeGenerator TypeGenerator => _proxyTypeGenerator;

        public object CreateClassProxy(Type serviceType, Type implementationType, object[] args)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            var proxyType = _proxyTypeGenerator.CreateClassProxyType(serviceType, implementationType);
            var proxyArgs = new object[args.Length + 1];
            proxyArgs[0] = _activatorFactory;
            for (var i = 0; i < args.Length; i++)
            {
                proxyArgs[i + 1] = args[i];
            }
            return System.Activator.CreateInstance(proxyType, proxyArgs);
        }

        public object CreateInterfaceProxy(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            var proxyType = _proxyTypeGenerator.CreateInterfaceProxyType(serviceType);
            return System.Activator.CreateInstance(proxyType, _activatorFactory);
        }

        public object CreateInterfaceProxy(Type serviceType, object implementationInstance)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (implementationInstance == null)
            {
                return CreateInterfaceProxy(serviceType);
            }
            var proxyType = _proxyTypeGenerator.CreateInterfaceProxyType(serviceType, implementationInstance.GetType());
            return System.Activator.CreateInstance(proxyType, _activatorFactory, implementationInstance);
        }

        public void Dispose()
        {
        }
    }
}