using Acb.Core;
using Acb.Core.Extensions;
using Consul;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Register
{
    internal class ConsulRegister : IRegister
    {
        private static readonly List<string> Services = new List<string>();
        private readonly MicroServiceConfig _config;

        public ConsulRegister(MicroServiceConfig config)
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

        public async Task Regist(HashSet<Assembly> asses)
        {
            using (var client = GetClient())
            {
                foreach (var ass in asses)
                {
                    var assName = ass.GetName();
                    var serviceName = assName.Name;
                    var service = new AgentServiceRegistration
                    {
                        ID = $"{serviceName}_{_config.Host}_{_config.Port}".Md5(),
                        Name = serviceName,
                        Tags = new[] { Consts.Mode.ToString(), assName.Version.ToString() },
                        EnableTagOverride = true,
                        Address = $"http://{_config.Host}",
                        Port = _config.Port
                    };
                    if (_config.ConsulCheck)
                    {
                        service.Check = new AgentServiceCheck
                        {
                            HTTP = $"{service.Address}:{service.Port}/healthy",
                            Interval = TimeSpan.FromSeconds(_config.Consulinterval),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(_config.DeregisterAfter)
                        };
                    }
                    Services.Add(service.ID);
                    await client.Agent.ServiceRegister(service);
                }
            }
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
