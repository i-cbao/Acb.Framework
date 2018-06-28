using Acb.Core.Dependency;
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
        private readonly RedisManager _manager;
        public RedisTest()
        {
            _manager = CurrentIocManager.Resolve<RedisManager>();
        }
        [TestMethod]
        public void GetTest()
        {
            ThreadPool.GetMinThreads(out var min, out var compt);
            Print($"最小线程数：{min},{compt}");
            //ThreadPool.SetMinThreads(300, compt);
            const string key = "shay";
            var redis = _manager.GetDatabase("local", 4);
            redis.Set(key, "test", TimeSpan.FromMinutes(2));
            var result = CodeTimer.Time("redis test", 100, () =>
            {
                try
                {
                    var ritem = _manager.GetDatabase("local", 4);
                    var tmp = ritem.Get<string>(key);
                }
                catch (Exception ex)
                {
                    //Print(ex.Format());
                    throw;
                }
            }, 100);
            Print(result.ToString());
            var value = redis.Get<string>(key);
            Print(value);
        }

        [TestMethod]
        public void IncrementTest()
        {
            var t = _manager.GetDatabase("test");
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
            var t = _manager.GetSubscriber();
            const string chnl = "Subscriber_Test";

            t.Subscribe(chnl, (channel, value) => Print(value.ToString()));

            t.Publish(chnl, "shay");
            Thread.Sleep(2000);
        }
    }
}
