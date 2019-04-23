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
    public class ConsulRouter : IServiceRouter
    {
        private static readonly List<string> Services = new List<string>();
        private readonly MicroServiceConfig _config;

        public ConsulRouter(MicroServiceConfig config)
        {
            _config = config;
        }

        private ConsulClient GetClient()
        {
            return new ConsulClient(cfg =>
            {
                cfg.Address = new Uri(_config.ConsulServer);
                if (!string.IsNullOrWhiteSpace(_config.ConsulToken))
                    cfg.Token = _config.ConsulToken;
            });
        }

        public async Task Regist(IEnumerable<Assembly> serviceAssemblies, ServiceAddress address)
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
                        Port = _config.Port
                    };
                    if (_config.ConsulCheck)
                    {
                        service.Check = new AgentServiceCheck
                        {
                            HTTP = $"{address}/healthy",
                            Interval = TimeSpan.FromSeconds(_config.Consulinterval),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(_config.DeregisterAfter)
                        };
                    }

                    Services.Add(service.ID);
                    await client.Agent.ServiceRegister(service);
                }
            }
        }

        public async Task<List<ServiceAddress>> Find(Type serviceType)
        {
            var ass = serviceType.Assembly;
            var addresses = new List<ServiceAddress>();
            using (var client = GetClient())
            {
                var serviceName = ass.ServiceName();
                var list = await client.Service(serviceName,
                    new object[] { Consts.Mode, ass.GetName().Version });
                var items = list.Response.Select(t => new ServiceAddress(t.ServiceAddress, t.ServicePort)).ToArray();
                addresses.AddRange(items);
                //开发环境 可调用测试环境的微服务
                if (Consts.Mode == ProductMode.Dev)
                {
                    list = await client.Service(serviceName, new object[] { ProductMode.Test, ass.GetName().Version });
                    items = list.Response.Select(t => new ServiceAddress(t.ServiceAddress, t.ServicePort)).ToArray();
                    addresses.AddRange(items);
                }
            }

            return addresses;
        }

        public async Task Deregist()
        {
            using (var client = GetClient())
            {
                foreach (var service in Services)
                {
                    await client.Agent.ServiceDeregister(service);
                }
            }
        }
    }
}
