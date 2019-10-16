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
        private const string HostEnvironmentName = "MICROSERVICE_HOST";
        private const string PortEnvironmentName = "MICROSERVICE_PORT";
        private const string AutoDeregistEnvironmentName = "AUTO_DEREGIST";
        internal ConcurrentDictionary<string, MethodInfo> Methods { get; }

        internal HashSet<Assembly> ServiceAssemblies { get; }
        internal MicroServiceConfig Config { get; private set; }
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

        private void LoadConfig()
        {
            Config = Constants.MicroSreviceKey.Config<MicroServiceConfig>();
            var host = HostEnvironmentName.Env();
            var port = PortEnvironmentName.Env(0);
            if (!string.IsNullOrWhiteSpace(host))
                Config.Host = host;
            if (port > 0)
                Config.Port = port;
            var autoDeregist = AutoDeregistEnvironmentName.Env<bool?>(null);
            if (autoDeregist.HasValue)
                Config.AutoDeregist = autoDeregist.Value;
        }


        /// <summary> 注册微服务 </summary>
        public void Regist()
        {
            InitServices();
            LoadConfig();
            var asses = ServiceAssemblies;
            if (asses == null || asses.IsNullOrEmpty() || string.IsNullOrWhiteSpace(Config?.Host) || Config?.Port <= 0)
                return;
            _logger.Info($"regist service {Config.Host}:{Config.Port},gzip:{Config.Gzip}");
            _serviceRouter.Regist(asses, new ServiceAddress(Config.Host, Config.Port));
        }

        /// <summary> 取消注册 </summary>
        public void Deregist()
        {
            if (Config.AutoDeregist)
                _serviceRouter.Deregist();
        }
    }
}
