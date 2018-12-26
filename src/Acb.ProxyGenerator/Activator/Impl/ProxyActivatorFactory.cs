using System;

namespace Acb.ProxyGenerator.Activator.Impl
{
    public sealed class ProxyActivatorFactory : IProxyActivatorFactory
    {
        private readonly IProxyContextFactory _aspectContextFactory;
        private readonly IProxyBuilderFactory _aspectBuilderFactory;

        public ProxyActivatorFactory(IProxyContextFactory aspectContextFactory, IProxyBuilderFactory aspectBuilderFactory)
        {
            _aspectContextFactory = aspectContextFactory ?? throw new ArgumentNullException(nameof(aspectContextFactory));
            _aspectBuilderFactory = aspectBuilderFactory ?? throw new ArgumentNullException(nameof(aspectBuilderFactory));
        }

        public IProxyActivator Create()
        {
            return new ProxyActivator(_aspectContextFactory, _aspectBuilderFactory);
        }
    }
}
