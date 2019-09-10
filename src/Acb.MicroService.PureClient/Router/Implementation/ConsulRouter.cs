using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.MicroService.PureClient.Router.Implementation
{
    public class ConsulRouter : IRouter
    {
        private readonly MicroServiceConfig _config;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ConsulRouter> _logger;

        public ConsulRouter(MicroServiceConfig config, ILogger<ConsulRouter> logger = null)
        {
            _cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
            _config = config;
            _logger = logger;
        }

        private ConsulClient GetClient()
        {
            var uri = new Uri(_config.ConsulServer);
            return new ConsulClient(uri, _config.ConsulToken);
        }

        private static string Key(Type serviceType)
        {
            return serviceType.Assembly.AssemblyKey();
        }

        public async Task<List<ServiceAddress>> Find(Type serviceType)
        {
            var key = Key(serviceType);
            if (_cache.TryGetValue(key, out var value) && value != null && value is List<ServiceAddress> services)
            {
                _logger.LogDebug($"find services {key} from cache");
                return services;
            }

            var client = GetClient();
            var serviceName = serviceType.Assembly.ServiceName();
            services = await client.GetServices(serviceName, _config.Mode.ToString());
            _cache.Set(key, services, TimeSpan.FromMinutes(10));
            return services;
        }

        public Task CleanCache(Type serviceType)
        {
            var key = Key(serviceType);
            _cache.Remove(key);
            _logger?.LogDebug($"clean services {key} from cache");
            return Task.CompletedTask;
        }
    }
}
