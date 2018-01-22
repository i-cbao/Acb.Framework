using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ConfigTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<ConfigTest>();

        [TestMethod]
        public void GetTest()
        {
            //Encoding.RegisterProvider();
            var encoding = Encoding.GetEncoding("gbk");
            Print("dfdfs电风扇地方".UrlEncode(encoding));
            //Assembly.GetExecutingAssembly();
            //var config = "dapper:mscreen".Config<ConnectionConfig>();
            //Print(config);
            //_logger.Debug(config);
            //var redis = "redis:default".Config<string>();
            //Print(redis);
            var redis = RedisManager.Instance.GetDatabase();
            redis.Set("shay", "test", TimeSpan.FromMinutes(2));
            var tasks = new List<Task>();
            const int thread = 10;
            const int preCount = 1000;
            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < thread; i++)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    var count = 0;
                    try
                    {
                        var redis1 = RedisManager.Instance.GetDatabase();
                        for (var j = 0; j < preCount; j++)
                        {
                            var tmp = redis1.Get<string>("shay");
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message, ex);
                    }

                    return count;
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            watch.Stop();
            var tps = Math.Round((thread * preCount * 1000D) / watch.ElapsedMilliseconds, 4);
            Print(
                $"{thread * preCount}次查询，耗时：{watch.ElapsedMilliseconds}(ms),TPS:{tps}(req/sec)");
            foreach (var task in tasks)
            {
                Print((task as Task<int>)?.Result ?? 0);
            }
            var value = redis.Get<string>("shay");
            Print(value);
            //_logger.Info(redis);
            //var mongo = "mongo:default".Config<MongoConfig>();
            //Print(mongo);
            //_logger.Warn(mongo);
            //try
            //{
            //    throw new BusiException("error ex");
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error(ex.Message, ex);
            //}
            //Print(Consts.Mode);
            //var name = "name".Config<string>();
            //Print(name);
            //_logger.Fatal(name);
            //name = "shay".Config<string>();
            //Print(name);
        }
    }
}
