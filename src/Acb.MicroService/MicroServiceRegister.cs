using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.MicroService.Register;
using System;

namespace Acb.MicroService
{
    /// <summary> 微服务注册 </summary>
    internal class MicroServiceRegister
    {
        public const string MicroSreviceKey = "micro_service";
        private const string HostEnvironmentName = "MICRO_SERVICE_HOST";
        private const string PortEnvironmentName = "MICRO_SERVICE_PORT";

        private static MicroServiceConfig _config;
        private static IRegister _register;

        static MicroServiceRegister()
        {
            LoadConfig();
            _register = GetRegister();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                Deregist();
                LoadConfig();
                _register = GetRegister();
                Regist();
            };
        }

        private static void LoadConfig()
        {
            _config = MicroSreviceKey.Config<MicroServiceConfig>();
            var host = Environment.GetEnvironmentVariable(HostEnvironmentName);
            var port = Environment.GetEnvironmentVariable(PortEnvironmentName).CastTo(0);
            if (!string.IsNullOrWhiteSpace(host))
                _config.Host = host;
            if (port > 0)
                _config.Port = port;
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
