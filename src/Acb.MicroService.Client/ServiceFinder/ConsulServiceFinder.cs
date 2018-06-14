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
        private readonly ICache _cache;
        private static readonly object LockObj = new object();

        public ConsulServiceFinder()
        {
            _cache = CacheManager.GetCacher<ConsulServiceFinder>();
        }

        private static ConsulClient GetClient(MicroServiceConfig config)
        {
            return new ConsulClient(cfg =>
            {
                cfg.Address = new Uri(config.ConsulServer);
                if (!string.IsNullOrWhiteSpace(config.ConsulToken))
                    cfg.Token = config.ConsulToken;
            });
        }

        public List<string> Find(Assembly ass, MicroServiceConfig config)
        {
            lock (LockObj)
            {
                var key = ass.AssemblyKey();
                var urls = _cache.Get<List<string>>(key);
                if (urls != null)
                    return urls;
                var name = ass.GetName();
                urls = new List<string>();
                using (var client = GetClient(config))
                {
                    var list = client.Catalog.Service(name.Name, $"{Consts.Mode}").Result;
                    var items = list.Response.Select(t => $"{t.ServiceAddress}:{t.ServicePort}/").ToArray();
                    urls.AddRange(items);
                    //开发环境 可调用测试环境的微服务
                    if (Consts.Mode == ProductMode.Dev)
                    {
                        list = client.Catalog.Service(name.Name, $"{ProductMode.Test}").Result;
                        items = list.Response.Select(t => $"{t.ServiceAddress}:{t.ServicePort}/").ToArray();
                        urls.AddRange(items);
                    }
                }

                _cache.Set(key, urls, TimeSpan.FromMinutes(5));
                return urls;
            }


        }
    }
}
