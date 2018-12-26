using Acb.ProxyGenerator.Attributes;
using System;

namespace Acb.ProxyGenerator
{
    public interface IProxy
    {
        object Proxy(Type interfaceType, AopAttribute aop = null);

        object Aop(Type serviceType, AopAttribute aop = null);
    }

    public static class ProxyExtensions
    {
        public static T Proxy<T>(this IProxy proxy, AopAttribute aop = null)
        where T : class
        {
            return (T)proxy.Proxy(typeof(T), aop);
        }

        public static T Aop<T>(this IProxy proxy, AopAttribute aop = null)
        {
            return (T)proxy.Aop(typeof(T), aop);
        }
    }
}
