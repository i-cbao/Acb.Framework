using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

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

        public static IServiceCollection AddProxy(IServiceCollection services)
        {
            //services.Add(provider =>
            //{
            //    var serviceType = typeof(IMicroService);
            //    var types = provider.GetService<ITypeFinder>()
            //        .Find(t => serviceType.IsAssignableFrom(t) && t != serviceType);
            //    foreach (var type in types)
            //    {
            //        services.AddSingleton(type, ProxyService.Proxy(type));
            //    }

            //    return services;
            //});
            return services;
        }
    }
}
