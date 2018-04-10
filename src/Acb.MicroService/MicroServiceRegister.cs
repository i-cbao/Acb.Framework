using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.MicroService.Register;

namespace Acb.MicroService
{
    /// <summary> 微服务注册 </summary>
    internal class MicroServiceRegister
    {
        public const string MicroSreviceKey = "micro_service";
        private static MicroServiceConfig _config;
        private static IRegister _register;

        static MicroServiceRegister()
        {
            _config = MicroSreviceKey.Config<MicroServiceConfig>();
            _register = GetRegister();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                Deregist();
                _config = MicroSreviceKey.Config<MicroServiceConfig>();
                _register = GetRegister();
                Regist();
            };
        }

        private static IRegister GetRegister()
        {
            switch (_config.Register)
            {
                case RegisterType.Consul:
                    return new ConsulRegister();
                case RegisterType.Redis:
                    return new RedisRegister();
                default:
                    return new RedisRegister();
            }
        }


        /// <summary> 注册微服务 </summary>
        public static void Regist()
        {
            MicroServiceRouter.InitServices();
            var asses = MicroServiceRouter.ServiceAssemblies;
            if (asses == null || asses.IsNullOrEmpty())
                return;
            _register.Regist(asses, _config);
        }

        /// <summary> 取消注册 </summary>
        public static void Deregist()
        {
            _register.Deregist();
        }
    }
}
