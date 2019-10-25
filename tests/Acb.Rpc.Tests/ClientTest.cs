using Acb.Market.Contracts;
using Acb.MicroService.PureClient;
using Acb.Plugin.PrivilegeManage.Constract;
using Dcp.Net.MQ.RpcDefaultSdk;
using Dynamic.Core.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Rpc.Tests
{
    [TestClass]
    public class ClientTest
    {
        public ClientTest()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFilter((category, level) => !category.StartsWith("System.") && !category.StartsWith("Microsoft."));
            });
            //注入配置
            services.AddMicroClient(config =>
            {
                //config.ConsulToken = "fc243b3a62ecd334";
                //config.Mode = ProductMode.Test;
                //config.Mode = ProductMode.Prod
            });
            //注入代理IOC
            //services.AddProxy<IVCodeContract>();
            IocUnity.AddBuild(services);
            IocUnity.Get<IServiceProvider>().UseMicroClient();
            //初始化组件
            RpcProxyFactory.InitDefault("H4sIAAAAAAAAC6tWCi1OLfJLzE1VslLKTE5S0lEKSCwuLs8vSoEIGJpaKNUCAD1OqK0mAAAA", "Dev"); //Online
        }

        private static void Print(object obj)
        {
            string content;
            if (obj == null)
                content = "NULL";
            else if (new[] { typeof(int), typeof(string), typeof(bool) }.Contains(obj.GetType()))
            {
                content = obj.ToString();
            }
            else
            {
                content = JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            Console.WriteLine(content);
        }

        [TestMethod]
        public void CodeTest()
        {
            //var c = Convert.FromBase64String(
            //    "H4sIAAAAAAAAC6tWCi1OLfJLzE1VslLKTE5S0lEKSCwuLs8vSoEIGJpaKNUCAD1OqK0mAAAA");
            //var t = Encoding.UTF8.GetString(c.UnZip().Result);
            //Print(t);
            //需注入IOC
            //var contract = IocUnity.Get<IVCodeContract>();
            var contract = ProxyService.Proxy<IVCodeContract>();
            var code = contract.Generate();
            Print(code);
            code = contract.Generate();
            Print(code);
            var brandContract = ProxyService.Proxy<Vehicle.Contracts.IBrandContract>();
            var list = brandContract.SearchVersionByNamesAsync(new List<string> { "长安", "吉利" }).Result;
            Print(list);
        }

        [TestMethod]
        public async Task OrgTest()
        {
            var proxy = RpcProxyFactory.CreateProxy<IPrivilegeManageConstract>();
            var orgs = await proxy.GetOrganizations(new List<string>() { "157889570d18c214115708d6aad07a21" });
            Print(orgs);
        }
    }
}
