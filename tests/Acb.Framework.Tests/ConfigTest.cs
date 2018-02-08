using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Dapper.Config;
using Acb.MongoDb;
using log4net.Appender;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ConfigTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<ConfigTest>();

        [TestMethod]
        public void GetTest()
        {
            var config = "dapper:default".Config<ConnectionConfig>();
            Print(config);
            _logger.Debug(config);
            var redis = "redis:default".Config<string>();
            Print(redis);
            _logger.Info(redis);
            var mongo = "mongo:default".Config<MongoConfig>();
            Print(mongo);
            _logger.Warn(mongo);
            try
            {
                throw new BusiException("error ex");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            Print(Consts.Mode);
            var name = "name".Config<string>();
            Print(name);
            _logger.Fatal(name);
        }

        [TestMethod]
        public void Log4NetConfigTest()
        {
            var config = "log4net:debug".Config<RollingFileAppender>();
            Print(config);
        }
    }
}
