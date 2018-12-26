using Acb.ProxyGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.ProxyGenerator.Impl
{
    [NonProxy]
    public sealed class ProxyBuilder : IProxyBuilder
    {
        private readonly IList<Func<ProxyDelegate, ProxyDelegate>> _delegates;
        private readonly ProxyDelegate _complete;
        private ProxyDelegate _aspectDelegate;

        public ProxyBuilder(ProxyDelegate complete, IList<Func<ProxyDelegate, ProxyDelegate>> delegates)
        {
            _complete = complete ?? throw new ArgumentNullException(nameof(complete));
            _delegates = delegates ?? new List<Func<ProxyDelegate, ProxyDelegate>>();
        }

        public IEnumerable<Func<ProxyDelegate, ProxyDelegate>> Delegates => _delegates;

        public void AddProxyDelegate(Func<ProxyContext, ProxyDelegate, Task> interceptorInvoke)
        {
            if (interceptorInvoke == null)
            {
                throw new ArgumentNullException(nameof(interceptorInvoke));
            }
            _delegates.Add(next => context => interceptorInvoke(context, next));
        }

        public ProxyDelegate Build()
        {
            if (_aspectDelegate != null)
            {
                return _aspectDelegate;
            }
            var invoke = _complete;
            var count = _delegates.Count;
            for (var i = count - 1; i > -1; i--)
            {
                invoke = _delegates[i](invoke);
            }
            return _aspectDelegate = invoke;
        }
    }
}