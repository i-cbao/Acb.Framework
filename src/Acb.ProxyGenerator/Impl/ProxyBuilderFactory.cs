using Acb.Core.Cache;
using System;
using System.Reflection;

namespace Acb.ProxyGenerator.Impl
{
    public sealed class ProxyBuilderFactory : IProxyBuilderFactory
    {
        private readonly ICache _caching;

        public ProxyBuilderFactory()
        {
            _caching = CacheManager.GetCacher<ProxyBuilderFactory>();
        }

        public IProxyBuilder Create(ProxyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var key = GetKey(context.ServiceMethod, context.ImplementationMethod);
            var builder = _caching.Get<IProxyBuilder>(key);
            if (builder != null)
                return builder;
            builder = Create(Tuple.Create(context.ServiceMethod, context.ImplementationMethod));
            _caching.Set(key, builder);
            return builder;
        }

        private IProxyBuilder Create(Tuple<MethodInfo, MethodInfo> tuple)
        {
            var builder = new ProxyBuilder(context => context.Complete(), null);

            //foreach (var interceptor in _interceptorCollector.Collect(tuple.Item1, tuple.Item2))
            //    builder.AddProxyDelegate(interceptor.Invoke);

            return builder;
        }

        private string GetKey(MethodInfo serviceMethod, MethodInfo implementationMethod)
        {
            return $"{serviceMethod.GetHashCode()},{implementationMethod.GetHashCode()}";
        }
    }
}