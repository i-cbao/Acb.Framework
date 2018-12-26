using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Demo.Business.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [TestMethod]
        public async Task TaskTest()
        {

            var t = Task.Run(() =>
            {
                throw new BusiException("busi msg");
                return "shay";
            });
            try
            {
                var w1 = t.SyncRun();
                Print(w1);
            }
            catch (Exception ex)
            {
                //BusiException
                Console.WriteLine(ex);
            }

            try
            {
                var w2 = t.Result;
                Print(w2);
            }
            catch (Exception e)
            {
                //AggregateException
                Console.WriteLine(e);
            }

            var act = new Action(() => Console.WriteLine("shay"));
            await act;
        }
    }
}
