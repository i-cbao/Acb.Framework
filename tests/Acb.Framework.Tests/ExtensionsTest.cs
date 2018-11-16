using Acb.Core.Extensions;
using Acb.Demo.Business.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ExtensionsTest : DTest
    {
        [TestMethod]
        public void CheckPropsTest()
        {
            var source = new
            {
                Id = "1234",
                CityName = "北京",
                ParentCode = "10010",
                Deep = 0
            };
            var target = new TAreas
            {
                Id = "12341",
                CityName = "北京市",
                ParentCode = "10010",
                Deep = 1
            };
            var props = source.CheckProps(target, reset: true);
            Print(props.Select(t => t.Name));
        }
    }
}
