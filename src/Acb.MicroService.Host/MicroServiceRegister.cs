using Acb.Core;
using Acb.Core.Config;
using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService.Host
{
    /// <summary> 微服务注册 </summary>
    internal class MicroServiceRegister
    {
        private const string HostEnvironmentName = "MICRO_SERVICE_HOST";
        private const string PortEnvironmentName = "MICRO_SERVICE_PORT";
        private const string AutoDeregistEnvironmentName = "AUTO_DEREGIST";
        internal static ConcurrentDictionary<string, MethodInfo> Methods { get; }

        internal static HashSet<Assembly> ServiceAssemblies { get; }


        private static MicroServiceConfig _config;

        static MicroServiceRegister()
        {
            Methods = new ConcurrentDictionary<string, MethodInfo>();
            ServiceAssemblies = new HashSet<Assembly>();
            LoadConfig();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                Deregist();
                LoadConfig();
                Regist();
            };
        }

        /// <summary> 初始化服务 </summary>
        internal static void InitServices()
        {
            var finder = CurrentIocManager.Resolve<ITypeFinder>();
            var serviceType = typeof(IMicroService);
            var services = finder.Find(t => serviceType.IsAssignableFrom(t) && t.IsInterface && t != serviceType)
                .ToList();
            foreach (var service in services)
            {
                //过滤本地未实现的微服务
                var resolved = finder.Find(t => service.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                if (!resolved.Any())
                    continue;
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
            _config = Constants.MicroSreviceKey.Config<MicroServiceConfig>();
            var host = HostEnvironmentName.Env();
            var port = PortEnvironmentName.Env(0);
            if (!string.IsNullOrWhiteSpace(host))
                _config.Host = host;
            if (port > 0)
                _config.Port = port;
            var autoDeregist = AutoDeregistEnvironmentName.Env<bool?>(null);
            if (autoDeregist.HasValue)
                _config.AutoDeregist = autoDeregist.Value;
        }


        /// <summary> 注册微服务 </summary>
        public static void Regist()
        {
            InitServices();
            var asses = ServiceAssemblies;
            if (asses == null || asses.IsNullOrEmpty() || string.IsNullOrWhiteSpace(_config?.Host) || _config?.Port <= 0)
                return;
            CurrentIocManager.Resolve<IServiceRouter>().Regist(asses, new ServiceAddress(_config.Host, _config.Port));
        }

        /// <summary> 取消注册 </summary>
        public static void Deregist()
        {
            if (_config.AutoDeregist)
                CurrentIocManager.Resolve<IServiceRouter>().Deregist();
        }
    }
}
