using Acb.Core.Extensions;
using Acb.Dapper.Config;
using Acb.MongoDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Acb.Core;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ConfigTest : DTest
    {
        public ConfigTest() : base(Assembly.GetExecutingAssembly())
        {
        }

        [TestMethod]
        public void GetTest()
        {
            var config = "dapper:mscreen".Config<ConnectionConfig>();
            Print(config);
            var redis = "redis:default".Config<string>();
            Print(redis);
            var mongo = "mongo:default".Config<MongoConfig>();
            Print(mongo);
            Print(Consts.Mode);
            var name = "name".Config<string>();
            Print(name);
            //name = "shay".Config<string>();
            //Print(name);
        }
    }
}
