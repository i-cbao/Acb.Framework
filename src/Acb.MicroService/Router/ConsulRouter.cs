using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Router
{
    public class ConsulRouter : DServiceRouter
    {
        private readonly List<string> _services;

        public ConsulRouter(MicroServiceConfig config) : base(config)
        {
            _services = new List<string>();
        }

        private ConsulClient GetClient()
        {
            return new ConsulClient(cfg =>
            {
                cfg.Address = new Uri(Config.ConsulServer);
                if (!string.IsNullOrWhiteSpace(Config.ConsulToken))
                    cfg.Token = Config.ConsulToken;
            });
        }

        public override async Task Regist(IEnumerable<Assembly> serviceAssemblies, ServiceAddress address)
        {
            using (var client = GetClient())
            {
                foreach (var ass in serviceAssemblies)
                {
                    var serviceName = ass.ServiceName();

                    var service = new AgentServiceRegistration
                    {
                        ID = $"{serviceName}_{address}".Md5(),
                        Name = serviceName,
                        Tags = new[] { Consts.Mode.ToString(), ass.GetName().Version.ToString() },
                        EnableTagOverride = true,
                        Address = address.Address(),
                        Port = address.Port
                    };
                    if (Config.ConsulCheck)
                    {
                        service.Check = new AgentServiceCheck
                        {
                            HTTP = $"{address}/healthy",
                            Interval = TimeSpan.FromSeconds(Config.Consulinterval),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(Config.DeregisterAfter)
                        };
                    }

                    _services.Add(service.ID);
                    await client.Agent.ServiceRegister(service);
                }
            }
        }

        protected override async Task<List<ServiceAddress>> FindServices(Assembly assembly)
        {
            var addresses = new List<ServiceAddress>();
            using (var client = GetClient())
            {
                var serviceName = assembly.ServiceName();
                var list = await client.Health.Service(serviceName, Consts.Mode.ToString());
                var items = list.Response.Select(t => new ServiceAddress(t.Service.Address, t.Service.Port)).ToArray();
                addresses.AddRange(items);
                //开发环境 可调用测试环境的微服务
                if (Consts.Mode == ProductMode.Dev)
                {
                    list = await client.Health.Service(serviceName, ProductMode.Test.ToString());
                    items = list.Response.Select(t => new ServiceAddress(t.Service.Address, t.Service.Port)).ToArray();
                    addresses.AddRange(items);
                }
            }

            return addresses;
        }

        public override async Task Deregist()
        {
            using (var client = GetClient())
            {
                foreach (var service in _services)
                {
                    await client.Agent.ServiceDeregister(service);
                }
            }
        }
    }
}
