using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Acb.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

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

        /// <summary> 添加微服务代理注入 </summary>
        /// <param name="services"></param>
        /// <param name="finder">类型查找器</param>
        /// <returns></returns>
        public static IServiceCollection AddMicroClient(this IServiceCollection services, ITypeFinder finder = null)
        {
            if (!services.IsRegisted<IServiceRouter>())
            {
                services.AddMicroRouter();
            }

            finder = finder ?? new DefaultTypeFinder { AssemblyFinder = new DAssemblyFinder() };
            var serviceType = typeof(IMicroService);
            var types = finder.Find(t => serviceType.IsAssignableFrom(t) && t.IsInterface && t != serviceType);
            foreach (var type in types)
            {
                //过滤本地实现的微服务
                var resolved = finder.Find(t => type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                if (resolved.Any())
                    continue;
                //注入单例模式
                services.TryAddSingleton(type, provider => ProxyService.Proxy(type));
            }
            return services;
        }
    }
}
