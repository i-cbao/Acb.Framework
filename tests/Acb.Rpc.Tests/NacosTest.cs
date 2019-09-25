using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nacos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nacos.Utilities;

namespace Acb.Rpc.Tests
{
    [TestClass]
    public class NacosTest
    {
        private readonly IServiceProvider _provider;
        public NacosTest()
        {
            var services = new ServiceCollection();
            services.AddNacos(opt =>
            {
                opt.DefaultTimeOut = 8;
                opt.Namespace = "17517830-be19-42e3-b172-47f286b379de";
                opt.ServerAddresses = new List<string> { "192.168.0.231:30767" };
                opt.ListenInterval = 1000;
            });

            _provider = services.BuildServiceProvider();
        }

        private T Resolve<T>()
        {
            return _provider.GetService<T>();
        }

        [TestMethod]
        public async Task ConfigTest()
        {
            var client = Resolve<INacosConfigClient>();
            var config = await client.GetConfigAsync(new GetConfigRequest
            {
                DataId = "maker",
                Group = "DEFAULT_GROUP"
            });
            Console.WriteLine(config);
            //            await client.AddListenerAsync(new AddListenerRequest
            //            {
            //                DataId = "maker",
            //                Callbacks = new List<Action<string>>
            //                {
            //                    Console.WriteLine
            //                }
            //            });
        }

        [TestMethod]
        public async Task NamingTest()
        {
            var client = Resolve<INacosNamingClient>();
            //var result = await client.CreateServiceAsync(new CreateServiceRequest
            //{
            //    //                GroupName = "DEFAULT_GROUP",
            //    ServiceName = "micro-demo"
            //});
            //Console.WriteLine(result);
            //var result = await client.RegisterInstanceAsync(new RegisterInstanceRequest
            //{
            //    ServiceName = "micro-demo",
            //    Ip = "http://localhost",
            //    Port = 5000,
            //    Weight = 1,
            //    GroupName = "DEFAULT_GROUP",
            //    NamespaceId = "17517830-be19-42e3-b172-47f286b379de",
            //    Ephemeral = false
            //});

            //var result = await client.GetServiceAsync(new GetServiceRequest
            //{
            //    ServiceName = "micro-demo",
            //    GroupName = "DEFAULT_GROUP",
            //    NamespaceId = "17517830-be19-42e3-b172-47f286b379de"
            //});
            var result = await client.ListInstancesAsync(new ListInstancesRequest
            {
                ServiceName = "micro-demo",
                GroupName = "DEFAULT_GROUP",
                NamespaceId = "17517830-be19-42e3-b172-47f286b379de",
                HealthyOnly = true
            });

            //var result = await client.RemoveInstanceAsync(new RemoveInstanceRequest
            //{
            //    ServiceName = "micro-demo",
            //    Ip = "http://localhost",
            //    Port = 5000,
            //    GroupName = "DEFAULT_GROUP",
            //    NamespaceId = "17517830-be19-42e3-b172-47f286b379de"
            //});
            Console.WriteLine(result.ToJsonString());
        }
    }
}
