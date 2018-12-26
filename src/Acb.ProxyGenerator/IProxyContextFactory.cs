using Acb.ProxyGenerator.Activator;
using Acb.ProxyGenerator.Activator.Impl;
using Acb.ProxyGenerator.Impl;

namespace Acb.ProxyGenerator
{
    public interface IProxyContextFactory
    {
        ProxyContext CreateContext(ProxyActivatorContext activatorContext);

        void ReleaseContext(ProxyContext aspectContext);
    }
}
