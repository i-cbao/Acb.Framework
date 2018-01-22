using Acb.Core.Cache;
using Acb.Core.Logging;
using Acb.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class RedisTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<ConfigTest>();
        [TestMethod]
        public void GetTest()
        {
            const string key = "shay";
            var redis = CacheManager.GetCacher("framework", CacheLevel.Both);

            redis.Set(key, "test", TimeSpan.FromMinutes(2));
            var result = CodeTimer.Time("redis test", 1000, () =>
            {
                var tmp = redis.Get<string>(key);
            }, 10);
            Print(result.ToString());
            var value = redis.Get<string>(key);
            Print(value);
        }
    }
}
