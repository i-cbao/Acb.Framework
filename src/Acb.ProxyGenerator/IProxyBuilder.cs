using System;
using System.Collections.Generic;

namespace Acb.ProxyGenerator
{
    public interface IProxyBuilder
    {
        IEnumerable<Func<ProxyDelegate, ProxyDelegate>> Delegates { get; }

        ProxyDelegate Build();
    }
}
