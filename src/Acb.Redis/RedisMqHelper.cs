using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Acb.Redis
{
    public abstract class RedisMqHelper
    {
        protected readonly ILogger Logger;
        private readonly IDatabase _redis;

        protected RedisMqHelper(string configName)
        {
            _redis = RedisManager.Instance.GetDatabase(configName);
            Logger = LogManager.Logger(GetType());
        }

        protected RedisMqHelper(IDatabase redis)
        {
            _redis = redis;
            Logger = LogManager.Logger(GetType());
        }

        protected async Task<bool> PopAsync(string redisKey, Func<RedisValue, Task> syncAction)
        {
            var json = await _redis.ListLeftPopAsync(redisKey);
            if (!json.HasValue || string.IsNullOrWhiteSpace(json))
                return false;
            try
            {
                await syncAction(json);
            }
            catch (Exception ex)
            {
                try
                {
                    //添加错误信息
                    var t = JsonConvert.DeserializeObject<JObject>(json);
                    t.Add("__error_msg", ex.Message);
                    if (ex is BusiException busi)
                    {
                        Logger.Info(busi.Message);
                        t.Add("__error_code", busi.Code);
                    }
                    else
                    {
                        Logger.Error(ex.Message, ex);
                        t.Add("__error_except", ex.Format());
                    }

                    await _redis.ListRightPushAsync(redisKey + "_fail", JsonConvert.SerializeObject(t));
                }
                catch
                {
                    await _redis.ListRightPushAsync(redisKey + "_fail", json);
                }
            }

            return true;
        }

        protected async Task LoopPopAsync(string redisKey, Func<RedisValue, Task> syncAction)
        {
            while (true)
            {
                var result = await PopAsync(redisKey, syncAction);
                if (!result)
                    break;
            }
        }
    }
}
