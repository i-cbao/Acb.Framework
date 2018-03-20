using Acb.Core.Cache;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Tests;
using Acb.Core.Timing;
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

        [TestMethod]
        public void IncrementTest()
        {
            var t = Redis.RedisManager.Instance.GetDatabase();
            const string key = "test";
            //if (!t.KeyExists(key))
            //{
            //    t.StringSet(key, RandomHelper.Random().Next(100), Clock.Now.Date.AddDays(1) - Clock.Now);
            //}

            var id = t.StringIncrement(key);
            t.KeyExpire(key, Clock.Now.Date.AddDays(1));
            Print(id);
        }
    }
}
