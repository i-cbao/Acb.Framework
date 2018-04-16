using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Acb.Redis;
using System;
using System.Collections.Generic;
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
            var urlList = new List<string>();
            var redis = RedisManager.Instance.GetDatabase();

            var list = redis.SetMembers($"{Constans.RegistCenterKey}:{Consts.Mode}:{assemblyKey}");
            urlList.AddRange(list.Select(t => t.ToString()));
            if (Consts.Mode == ProductMode.Dev)
            {
                list = redis.SetMembers($"{Constans.RegistCenterKey}:{ProductMode.Test}:{assemblyKey}");
                urlList.AddRange(list.Select(t => t.ToString()));
            }
            urls = urlList.ToArray();
            cache.Set(assemblyKey, urls, TimeSpan.FromMinutes(5));
            return urls;
        }
    }
}
