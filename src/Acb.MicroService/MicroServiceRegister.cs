using Acb.MicroService.Client;
using Acb.Redis;
using StackExchange.Redis;
using System.Linq;

namespace Acb.MicroService
{
    public class MicroServiceRegister
    {
        public static void Regist(string ip, int port)
        {
            var list = MicroServiceRouter.GetServices();
            var redis = RedisManager.Instance.GetDatabase(defaultDb: 2);

            redis.HashSet(Constants.RegistCenterKey,
                list.Select(t => new HashEntry(t.FullName, $"http://{ip}:{port}/")).ToArray());
        }
    }
}
