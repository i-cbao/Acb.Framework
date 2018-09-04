using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Acb.MicroService.Client
{
    public static class ProxyExtensions
    {
        /// <summary> 获取代理对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public static T Proxy<T>(this DService service) where T : IMicroService
        {
            return ProxyService.Proxy<T>();
        }

        public static IServiceCollection AddProxy(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var types = provider.GetService<ITypeFinder>()
                    .Find(t => t.IsAssignableFrom(typeof(IMicroService)) && t.IsInterface);
                var list = new List<object>();
                foreach (var type in types)
                {
                    list.Add(ProxyService.Proxy(type));
                }
                return list;
            });
            return services;
        }
    }
}
