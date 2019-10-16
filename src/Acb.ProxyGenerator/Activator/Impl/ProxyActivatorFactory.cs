using System;

namespace Acb.ProxyGenerator.Activator.Impl
{
    public sealed class ProxyActivatorFactory : IProxyActivatorFactory
    {
        private readonly IProxyContextFactory _contextFactory;
        private readonly IProxyBuilderFactory _builderFactory;

        public ProxyActivatorFactory(IProxyContextFactory contextFactory, IProxyBuilderFactory builderFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _builderFactory = builderFactory ?? throw new ArgumentNullException(nameof(builderFactory));
        }

        public IProxyActivator Create()
        {
            return new ProxyActivator(_contextFactory, _builderFactory);
        }
    }
}
