﻿using Acb.Core.Extensions;
using Consul;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acb.MicroService.Register
{
    internal class ConsulRegister : IRegister
    {
        private static readonly List<string> Services = new List<string>();
        private MicroServiceConfig _config;

        public void Regist(HashSet<Assembly> asses, MicroServiceConfig config)
        {
            _config = config;
            using (var client = new ConsulClient(cfg => cfg.Address = new Uri(config.ConsulServer)))
            {
                foreach (var ass in asses)
                {
                    var assName = ass.GetName();
                    var service = new AgentServiceRegistration
                    {
                        ID = $"{ass.AssemblyKey()}_{config.Host}_{config.Port}".Md5(),
                        Name = assName.Name,
                        Tags = new[] { assName.Version.ToString() },
                        EnableTagOverride = true,
                        Address = $"http://{config.Host}",
                        Port = config.Port
                    };
                    if (config.ConsulCheck)
                    {
                        service.Check = new AgentServiceCheck
                        {
                            HTTP = "/healthy",
                            Interval = TimeSpan.FromSeconds(config.Consulinterval),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(config.DeregisterAfter)
                        };
                    }
                    Services.Add(service.ID);
                    client.Agent.ServiceRegister(service).Wait();
                }
            }
        }

        public void Deregist()
        {
            using (var client = new ConsulClient(cfg => cfg.Address = new Uri(_config.ConsulServer)))
            {
                foreach (var service in Services)
                {
                    client.Agent.ServiceDeregister(service).Wait();
                }
            }
        }
    }
}
