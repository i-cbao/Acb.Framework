using Acb.Core;
using Acb.Core.Domain;
using Acb.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acb.Core.Dependency;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal class RedisServiceFinder : IServiceFinder
    {
        public List<string> Find(Assembly ass, MicroServiceConfig config)
        {
            var urlList = new List<string>();
            var redis = CurrentIocManager.Resolve<RedisManager>().GetDatabase();

            var list = redis.SetMembers($"{Constans.RegistCenterKey}:{Consts.Mode}:{ass.GetName().Name}");
            urlList.AddRange(list.Select(t => t.ToString()));
            if (Consts.Mode == ProductMode.Dev)
            {
                list = redis.SetMembers($"{Constans.RegistCenterKey}:{ProductMode.Test}:{ass.GetName().Name}");
                urlList.AddRange(list.Select(t => t.ToString()));
            }

            return urlList;
        }
    }
}
