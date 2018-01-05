using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class HelperTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<HelperTest>();

        public HelperTest() : base(Assembly.GetExecutingAssembly())
        {
        }

        [TestMethod]
        public void Md5Test()
        {
            var md5 = "shay".Md5();
            _logger.Info(md5);
            Assert.AreEqual(md5.Length, 32);
        }
    }
}
