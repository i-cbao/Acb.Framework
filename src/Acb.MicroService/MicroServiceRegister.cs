using Acb.Core.Extensions;
using Acb.Redis;
using System.Reflection;

namespace Acb.MicroService
{
    /// <summary> 微服务注册 </summary>
    internal class MicroServiceRegister
    {
        private const string MicroSreviceKey = "micro_service";
        private const string RegistCenterKey = MicroSreviceKey + ":center";

        private static string TypeUrl(MemberInfo type)
        {
            var config = MicroSreviceKey.Config<MicroServiceConfig>();
            return $"http://{config.Host}:{config.Port}/{type.Name}/";
        }

        /// <summary> 注册 </summary>
        public static void Regist()
        {
            var list = MicroServiceRouter.GetServices();
            if (list == null || list.IsNullOrEmpty())
                return;


            var redis = RedisManager.Instance.GetDatabase();
            foreach (var type in list)
            {
                redis.SetAdd($"{RegistCenterKey}:{type.FullName}", TypeUrl(type));
            }
        }

        /// <summary> 取消注册 </summary>
        public static void UnRegist()
        {
            var list = MicroServiceRouter.GetServices();
            if (list == null || list.IsNullOrEmpty())
                return;
            var redis = RedisManager.Instance.GetDatabase();
            foreach (var type in list)
            {
                redis.SetRemove($"{RegistCenterKey}:{type.FullName}", TypeUrl(type));
            }
        }
    }
}
