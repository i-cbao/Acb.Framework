using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Redis;
using System.Reflection;

namespace Acb.MicroService
{
    /// <summary> 微服务注册 </summary>
    internal class MicroServiceRegister
    {
        private const string MicroSreviceKey = "micro_service";
        private const string RegistCenterKey = MicroSreviceKey + ":center";
        private MicroServiceConfig _config;
        private string RedisKey => string.IsNullOrWhiteSpace(_config.RedisKey) ? RegistCenterKey : _config.RedisKey;

        private MicroServiceRegister()
        {
            _config = MicroSreviceKey.Config<MicroServiceConfig>();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                UnRegist();
                _config = MicroSreviceKey.Config<MicroServiceConfig>();
                Regist();
            };
        }

        public static MicroServiceRegister Instance => Singleton<MicroServiceRegister>.Instance ??
                                                       (Singleton<MicroServiceRegister>.Instance =
                                                           new MicroServiceRegister());


        private string TypeUrl(MemberInfo type)
        {
            return $"http://{_config.Host}:{_config.Port}/{type.Name}/";
        }

        /// <summary> 注册 </summary>
        public void Regist()
        {
            var list = MicroServiceRouter.GetServices();
            if (list == null || list.IsNullOrEmpty())
                return;
            var redis = RedisManager.Instance.GetDatabase();
            foreach (var type in list)
            {
                redis.SetAdd($"{RedisKey}:{type.FullName}", TypeUrl(type));
            }
        }

        /// <summary> 取消注册 </summary>
        public void UnRegist()
        {
            var list = MicroServiceRouter.GetServices();
            if (list == null || list.IsNullOrEmpty())
                return;
            var redis = RedisManager.Instance.GetDatabase();
            foreach (var type in list)
            {
                redis.SetRemove($"{RedisKey}:{type.FullName}", TypeUrl(type));
            }
        }
    }
}
