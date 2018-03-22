using System.Linq;
using Acb.Redis;

namespace Acb.MicroService.Test
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
