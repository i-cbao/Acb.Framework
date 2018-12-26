using Acb.ProxyGenerator.Activator;
using System;

namespace Acb.ProxyGenerator.Impl
{
    public sealed class ProxyContextFactory : IProxyContextFactory
    {
        private static readonly object[] EmptyParameters = new object[0];
        private readonly IServiceProvider _serviceProvider;

        public ProxyContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ProxyContext CreateContext(ProxyActivatorContext activatorContext)
        {
            return new RuntimeProxyContext(_serviceProvider,
                activatorContext.ServiceMethod,
                activatorContext.TargetMethod,
                activatorContext.ProxyMethod,
                activatorContext.TargetInstance,
                activatorContext.ProxyInstance,
                activatorContext.Parameters ?? EmptyParameters);
        }

        public void ReleaseContext(ProxyContext aspectContext)
        {
            (aspectContext as IDisposable)?.Dispose();
        }
    }
}