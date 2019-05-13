using Acb.Core.Cache;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Router
{
    /// <summary> 服务路由(注册/发现) </summary>
    public abstract class DServiceRouter : IServiceRouter
    {
        /// <summary> 服务配置 </summary>
        protected readonly MicroServiceConfig Config;
        /// <summary> 日志 </summary>
        protected readonly ILogger Logger;
        private readonly ICache _cache;

        protected DServiceRouter(MicroServiceConfig config)
        {
            Config = config;
            _cache = CacheManager.GetCacher(GetType());
            Logger = LogManager.Logger(GetType());
        }

        /// <summary> 注册服务 </summary>
        /// <param name="serviceAssemblies"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public abstract Task Regist(IEnumerable<Assembly> serviceAssemblies, ServiceAddress address);

        /// <summary> 服务发现 </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public async Task<List<ServiceAddress>> Find(Type serviceType)
        {
            var key = serviceType.Assembly.AssemblyKey();

            var addresses = _cache.Get<List<ServiceAddress>>(key);
            if (addresses != null && addresses.Any())
            {
                Logger.Debug($"find service {serviceType.Assembly.ServiceName()} from cache");
                return addresses;
            }

            addresses = await FindServices(serviceType.Assembly);
            if (addresses != null && addresses.Any())
                _cache.Set(key, addresses, TimeSpan.FromMinutes(5));
            Logger.Debug($"find service {serviceType.Assembly.ServiceName()} from center");
            return addresses;
        }

        protected abstract Task<List<ServiceAddress>> FindServices(Assembly assembly);

        public Task CleanCache(Type serviceType)
        {
            _cache.Remove(serviceType.Assembly.AssemblyKey());
            Logger.Debug($"clean service {serviceType.Assembly.ServiceName()} from cache");
            return Task.CompletedTask;
        }

        public abstract Task Deregist();
    }
}
