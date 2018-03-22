using Acb.Core.Extensions;
using Acb.MicroService.Client;
using Acb.Redis;
using StackExchange.Redis;
using System.Linq;

namespace Acb.MicroService
{
    public class MicroServiceRegister
    {
        public static void Regist()
        {
            var list = MicroServiceRouter.GetServices();
            var redis = RedisManager.Instance.GetDatabase(defaultDb: 2);
            var ip = "service_host".Config<string>();
            var port = "service_port".Config<int>();
            redis.HashSet(Constants.RegistCenterKey,
                list.Select(t => new HashEntry(t.FullName, $"http://{ip}:{port}/")).ToArray());
        }
    }
}
