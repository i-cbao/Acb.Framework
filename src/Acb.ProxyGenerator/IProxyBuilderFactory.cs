using Acb.ProxyGenerator.Impl;

namespace Acb.ProxyGenerator
{
    public interface IProxyBuilderFactory
    {
        IProxyBuilder Create(ProxyContext context);
    }
}
