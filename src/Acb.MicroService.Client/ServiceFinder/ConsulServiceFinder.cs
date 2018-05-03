using Acb.Core;
using Acb.Core.Domain;
using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal class ConsulServiceFinder : IServiceFinder
    {
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
            var name = ass.GetName();
            var urlList = new List<string>();
            using (var client = GetClient(config))
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

            return urlList;
        }
    }
}
