using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Tests;
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
        private readonly IDemoService _microService;
        private readonly IDemoService _localService;

        public MicroServiceTest()
        {
            _microService = ProxyService.Proxy<IDemoService>();
            LogManager.SetLevel(LogLevel.Off);
            _localService = CurrentIocManager.Resolve<IDemoService>();
        }

        [TestMethod]
        public void Test()
        {
            _microService.Load("shay");
            var task = _microService.LoadAsync();
            var dict = _microService.Dict(new[] { "123", "456" });
            Print(dict);
            var result = CodeTimer.Time("micro", 2000, () =>
            {
                var word = _microService.Hello(IdentityHelper.Guid32, new DemoInputDto
                {
                    Demo = DemoEnums.Test,
                    Name = "shay" + IdentityHelper.Guid16,
                    Time = Clock.Now
                });
                //Print(word);
            }, 10);
            Print(result.ToString());
            result = CodeTimer.Time("local", 2000, () =>
            {
                var word = _localService.Hello(IdentityHelper.Guid32, new DemoInputDto
                {
                    Demo = DemoEnums.Test,
                    Name = "shay" + IdentityHelper.Guid16,
                    Time = Clock.Now
                });
                //Print(word);
            }, 10);
            Print(result.ToString());
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
