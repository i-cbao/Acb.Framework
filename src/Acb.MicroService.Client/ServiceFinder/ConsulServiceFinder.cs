using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal class ConsulServiceFinder : IServiceFinder
    {
        public string[] Find(Assembly ass, MicroServiceConfig config)
        {
            var cache = CacheManager.GetCacher(typeof(InvokeProxy<>));
            var assemblyKey = ass.AssemblyKey();
            var name = ass.GetName();
            var urls = cache.Get<string[]>(assemblyKey);
            if (urls != null)
                return urls;
            var urlList = new List<string>();
            using (var client = new ConsulClient(cfg => cfg.Address = new Uri(config.ConsulServer)))
            {
                var list = client.Catalog.Service(name.Name, $"{Consts.Mode}_{name.Version}").Result;
                var items = list.Response.Select(t => $"{t.ServiceAddress}:{t.ServicePort}/").ToArray();
                urlList.AddRange(items);
                //开发环境 可调用测试环境的微服务
                if (Consts.Mode == ProductMode.Dev)
                {
                    list = client.Catalog.Service(name.Name, $"{ProductMode.Test}_{name.Version}").Result;
                    items = list.Response.Select(t => $"{t.ServiceAddress}:{t.ServicePort}/").ToArray();
                    urlList.AddRange(items);
                }
            }

            urls = urlList.ToArray();
            cache.Set(assemblyKey, urls, TimeSpan.FromMinutes(5));

            return urls;
        }
    }
}
