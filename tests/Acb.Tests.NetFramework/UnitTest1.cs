using Acb.Configuration;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.MicroService.Client;
using Acb.User.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Acb.Tests.NetFramework
{
    [TestClass]
    public class UnitTest1
    {
        private readonly IUserService _userService;
        public UnitTest1()
        {
            var module = new ConfigurationModule();
            module.Initialize();
            _userService = ProxyService.Proxy<IUserService>();
            LogManager.AddAdapter(new ConsoleAdapter());
        }

        [TestMethod]
        public void TestMethod1()
        {
            var dto = _userService.Detail("10031e5ceec348f486856f136d861e6f");
            Console.WriteLine(JsonHelper.ToJson(dto));
        }
    }
}
