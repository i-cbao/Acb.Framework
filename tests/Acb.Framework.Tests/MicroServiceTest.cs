using Acb.Core.Timing;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.Enums;
using Acb.MicroService.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class MicroServiceTest : DTest
    {
        [TestMethod]
        public void Test()
        {
            var demo = ProxyService.Proxy<IDemoService>();
            var word = demo.Hello(new DemoInputDto
            {
                Demo = DemoEnums.Test,
                Name = "shay001",
                Time = Clock.Now
            });
            Print(word);
        }
    }
}
