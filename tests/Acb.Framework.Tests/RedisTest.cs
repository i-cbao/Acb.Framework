using Acb.Core.Cache;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Tests;
using Acb.Core.Timing;
using Acb.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

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
            var t = RedisManager.Instance.GetDatabase("test");
            const string key = "icb10";
            if (!t.KeyExists(key))
            {
                t.StringSet(key, RandomHelper.Random().Next(100), Clock.Now.Date.AddDays(1) - Clock.Now);
            }

            t.StringSet("shay", IdentityHelper.Guid16);

            Print(t.StringGet("shay").ToString());
            //Print(t.StringGet(key).ToString());
            var id = t.StringIncrement(key);
            t.KeyExpire(key, Clock.Now.Date.AddDays(1));
            Print(id);            
        }

        [TestMethod]
        public void SubscribeTest()
        {
            var t = RedisManager.Instance.GetSubscriber();
            const string chnl = "Subscriber_Test";

            t.Subscribe(chnl, (channel, value) => Print(value.ToString()));

            t.Publish(chnl, "shay");
            Thread.Sleep(2000);
        }
    }
}
