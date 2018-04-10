using Acb.Core.Cache;
using Acb.Core.Extensions;
using Acb.MicroService.Client.ServiceFind;
using Acb.Redis;
using System;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal class RedisServiceFinder : IServiceFinder
    {
        public string[] Find(Assembly ass, MicroServiceConfig config)
        {
            var assemblyKey = ass.AssemblyKey();
            var cache = CacheManager.GetCacher(typeof(InvokeProxy<>));
            var urls = cache.Get<string[]>(assemblyKey);
            if (urls != null)
                return urls;
            var redis = RedisManager.Instance.GetDatabase();
            var key = string.IsNullOrWhiteSpace(config.RedisKey) ? Constans.RegistCenterKey : config.RedisKey;
            var list = redis.SetMembers($"{key}:{assemblyKey}");
            urls = list.Select(t => t.ToString()).ToArray();
            cache.Set(assemblyKey, urls, TimeSpan.FromMinutes(5));
            return urls;
        }
    }
}
