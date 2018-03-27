using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.Enums;
using Acb.MicroService.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class MicroServiceTest : DTest
    {
        //private readonly IMarketContract _marketContract;

        //public MicroServiceTest()
        //{
        //    _marketContract = ProxyService.Proxy<IMarketContract>();
        //}

        [TestMethod]
        public void Test()
        {
            var demo = ProxyService.Proxy<IDemoService>();
            var word = demo.Hello(IdentityHelper.Guid32, new DemoInputDto
            {
                Demo = DemoEnums.Test,
                Name = "shay001",
                Time = Clock.Now
            });
            Print(word);
        }

        [TestMethod]
        public void JsonTest()
        {
            const string json = "[510100]";
            var array = JsonConvert.DeserializeObject<JArray>(json);
            var value = array[0].Value<string>().CastTo(typeof(int));
            Print(value);
        }
        //[TestMethod]
        //public void LifeServiceTest()
        //{
        //    var dtos = _marketContract.GetLifeService("510100");
        //    Print(dtos);
        //    Assert.AreNotEqual(dtos, null);
        //}

        //[TestMethod]
        //public void VehiceIllegalsTest()
        //{
        //    const string vehiceId = "648fc3fac84641f69a8c2dd6c2306f84";
        //    var dtos = _marketContract.GetVehiceIllegals(vehiceId, 1, 10);
        //    Print(dtos);
        //    Assert.AreNotEqual(dtos, null);
        //}

    }
}
