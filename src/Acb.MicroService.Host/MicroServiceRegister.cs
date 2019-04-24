using Acb.Core;
using Acb.Core.Config;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.MicroService.Host
{
    /// <summary> 微服务注册 </summary>
    public class MicroServiceRegister
    {
        private const string HostEnvironmentName = "MICRO_SERVICE_HOST";
        private const string PortEnvironmentName = "MICRO_SERVICE_PORT";
        private const string AutoDeregistEnvironmentName = "AUTO_DEREGIST";
        internal ConcurrentDictionary<string, MethodInfo> Methods { get; }

        internal HashSet<Assembly> ServiceAssemblies { get; }
        private static MicroServiceConfig _config;
        private readonly IServiceRouter _serviceRouter;
        private readonly ITypeFinder _typeFinder;
        private readonly ILogger _logger;

        public MicroServiceRegister(IServiceRouter serviceRouter, ITypeFinder typeFinder)
        {
            _serviceRouter = serviceRouter;
            _typeFinder = typeFinder;
            _logger = LogManager.Logger<MicroServiceRegister>();
            Methods = new ConcurrentDictionary<string, MethodInfo>();
            ServiceAssemblies = new HashSet<Assembly>();

            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                _logger.Info("config changed");
                Deregist();
                Regist();
            };
        }

        /// <summary> 初始化服务 </summary>
        private void InitServices()
        {
            var serviceType = typeof(IMicroService);
            var services = _typeFinder.Find(t => serviceType.IsAssignableFrom(t) && t.IsInterface && t != serviceType)
                .ToList();
            foreach (var service in services)
            {
                //过滤本地未实现的微服务
                var resolved = _typeFinder.Find(t => service.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
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
        public void Regist()
        {
            InitServices();
            LoadConfig();
            var asses = ServiceAssemblies;
            if (asses == null || asses.IsNullOrEmpty() || string.IsNullOrWhiteSpace(_config?.Host) || _config?.Port <= 0)
                return;
            _logger.Info($"regist service {_config.Host}:{_config.Port}");
            _serviceRouter.Regist(asses, new ServiceAddress(_config.Host, _config.Port));
        }

        /// <summary> 取消注册 </summary>
        public void Deregist()
        {
            if (_config.AutoDeregist)
                _serviceRouter.Deregist();
        }
    }
}
