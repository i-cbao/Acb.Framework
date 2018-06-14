using Consul;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ConsulTest : DTest
    {
        [TestMethod]
        public async Task KvTest()
        {
            using (var client = new ConsulClient(cfg =>
            {
                cfg.Address = new Uri("http://192.168.0.252");
            }))
            {
                //服务注册
                //var result = await client.Agent.ServiceRegister(new AgentServiceRegistration
                //{
                //    ID = "test3",
                //    Name = "shay_service",
                //    Address = "http://192.168.0.252",
                //    Port = 1234,
                //    Check = new AgentServiceCheck
                //    {
                //        HTTP = "http://192.168.0.252",
                //        Interval = TimeSpan.FromSeconds(20),
                //        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                //    }
                //});
                //服务注销
                var result = await client.Agent.ServiceDeregister("66a6814a10633a1a");

                Print(result);
                //服务发现
                var services = await client.Catalog.Service("shay_service");
                Print(services.Response);
                var t = await client.KV.Get("dapper_default");
                Print(t.Response);
                Print(Encoding.UTF8.GetString(t.Response.Value));
            }
        }
    }
}
