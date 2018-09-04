using Acb.Core;
using Acb.MicroService.Client.Proxy;
using System;

namespace Acb.MicroService.Client
{
    /// <summary> 代理服务 </summary>
    public static class ProxyService
    {
        public static object Proxy(Type interfaceType)
        {
            var type = typeof(HttpProxy<>).MakeGenericType(interfaceType);
            return ProxyAsync.Create(type, interfaceType);
        }

        /// <summary> 生成代理 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Proxy<T>() where T : IMicroService
        {
            return ProxyAsync.Create<T, HttpProxy<T>>();
            //return ProxyAsync.Create<T, NettyProxy<T>>();
        }
    }
}
