using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Reflection;
using Acb.MicroService.Register;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService
{
    /// <summary> 微服务注册 </summary>
    internal class MicroServiceRegister
    {
        public const string MicroSreviceKey = "micro_service";
        private const string HostEnvironmentName = "MICRO_SERVICE_HOST";
        private const string PortEnvironmentName = "MICRO_SERVICE_PORT";
        internal static ConcurrentDictionary<string, MethodInfo> Methods { get; }

        internal static HashSet<Assembly> ServiceAssemblies { get; }


        private static MicroServiceConfig _config;
        private static IRegister _register;

        static MicroServiceRegister()
        {
            Methods = new ConcurrentDictionary<string, MethodInfo>();
            ServiceAssemblies = new HashSet<Assembly>();
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

        /// <summary> 初始化服务 </summary>
        internal static void InitServices()
        {
            var services = CurrentIocManager.Resolve<ITypeFinder>()
                .Find(t => typeof(IMicroService).IsAssignableFrom(t) && t.IsInterface && t != typeof(IMicroService))
                .ToList();
            foreach (var service in services)
            {
                if (!ServiceAssemblies.Contains(service.Assembly))
                    ServiceAssemblies.Add(service.Assembly);
                var methods = service.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    Methods.TryAdd($"{service.Name}/{method.Name}".ToLower(), method);
                }
            }
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
            InitServices();
            var asses = ServiceAssemblies;
            if (asses == null || asses.IsNullOrEmpty() || string.IsNullOrWhiteSpace(_config?.Host) || _config?.Port <= 0)
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
