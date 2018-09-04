using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.Proxy
{
    public abstract class ProxyAsync
    {
        public static object Create(Type proxyType, Type interfaceType)
        {
            return AsyncDispatchProxyGenerator.CreateProxyInstance(proxyType, interfaceType);
        }
        public static T Create<T, TProxy>() where TProxy : ProxyAsync
        {
            return (T)AsyncDispatchProxyGenerator.CreateProxyInstance(typeof(TProxy), typeof(T));
        }

        public abstract object Invoke(MethodInfo method, object[] args);

        public abstract Task InvokeAsync(MethodInfo method, object[] args);

        public abstract Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args);
    }
}
