using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Reflection;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

        internal static async Task<QueryResult<CatalogService[]>> Service(this ConsulClient client, string name,
            object[] tags, CancellationToken ct = default(CancellationToken))
        {
            var url = new Uri(client.Config.Address, $"/v1/catalog/service/{name}").AbsoluteUri;
            var ps = new List<string>();
            if (!string.IsNullOrWhiteSpace(client.Config.Token))
            {
                ps.Add($"token={client.Config.Token.UrlEncode()}");
            }

            ps.AddRange(tags.Where(t => t != null).Select(tag => $"tag={tag.ToString().UrlEncode()}"));

            if (ps.Any())
            {
                url += "?" + string.Join("&", ps);
            }

            var watcher = Stopwatch.StartNew();
            var resp = await HttpHelper.Instance.RequestAsync(HttpMethod.Get, new HttpRequest(url));
            watcher.Stop();
            var result =
                new QueryResult<CatalogService[]> { StatusCode = resp.StatusCode, RequestTime = watcher.Elapsed };

            if (!resp.IsSuccessStatusCode)
                return result;
            var content = await resp.Content.ReadAsStringAsync();
            result.Response = JsonConvert.DeserializeObject<CatalogService[]>(content);
            return result;

        }
    }
}
