using System;
using System.Linq;
using System.Reflection;
using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Extensions;
using Consul;

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
            using (var client = new ConsulClient(cfg => cfg.Address = new Uri(config.ConsulServer)))
            {
                var list = client.Catalog.Service(name.Name, $"{Consts.Mode}_{name.Version}").Result;
                urls = list.Response.Select(t => $"{t.ServiceAddress}:{t.ServicePort}/").ToArray();
                cache.Set(assemblyKey, urls, TimeSpan.FromMinutes(5));
            }

            return urls;
        }
    }
}
