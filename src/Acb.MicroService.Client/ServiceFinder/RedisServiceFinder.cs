using Acb.Core;
using Acb.Core.Domain;
using Acb.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.ServiceFinder
{
    internal class RedisServiceFinder : IServiceFinder
    {
        public async Task<List<string>> Find(Assembly ass, MicroServiceConfig config)
        {
            var urlList = new List<string>();
            var redis = RedisManager.Instance.GetDatabase();

            var list = await redis.SetMembersAsync($"{Constans.RegistCenterKey}:{Consts.Mode}:{ass.GetName().Name}");
            urlList.AddRange(list.Select(t => t.ToString()));
            if (Consts.Mode == ProductMode.Dev)
            {
                list = await redis.SetMembersAsync(
                    $"{Constans.RegistCenterKey}:{ProductMode.Test}:{ass.GetName().Name}");
                urlList.AddRange(list.Select(t => t.ToString()));
            }

            return urlList;
        }
    }
}
