﻿using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Demo.Business.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Acb.Core.Domain;

namespace Acb.Framework.Tests
{
    public enum EnumType
    {
        [System.ComponentModel.Description("AA")]
        A = 1,
        [System.ComponentModel.Description("BB")]
        B = 3
    }

    [TestClass]
    public class ExtensionsTest : DTest
    {
        [TestMethod]
        public void EnumTest()
        {
            var text = (EnumType.A | EnumType.B).GetText(true);
            Print(text);
        }

        [TestMethod]
        public void CheckPropsTest()
        {
            var source = new
            {
                Id = "1234",
                CityName = "北京",
                //ParentCode = "10010",
                Deep = 0
            };
            var target = new TAreas
            {
                Id = "12341",
                CityName = "北京市",
                ParentCode = "10010",
                Deep = 1
            };
            var props = source.CheckProps(target, reset: true, setValue: true);
            Print(props.Select(t => t.Name));
            Print(target);
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

            //Resolve<IUnitOfWork>().Trans(async () =>
            //{
            //    await Task.FromResult(0);
            //});

            var act = new Action(() => Console.WriteLine("shay"));
            await act;
        }
    }
}
