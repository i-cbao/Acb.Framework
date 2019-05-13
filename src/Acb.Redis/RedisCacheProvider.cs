using Acb.Core.Cache;
using System;
using System.Collections.Concurrent;

namespace Acb.Redis
{
    /// <summary> Redis提供者 </summary>
    public class RedisCacheProvider : ICacheProvider
    {
        private static readonly ConcurrentDictionary<string, Lazy<ICache>> Caches;
        private readonly RedisConfig _config;

        static RedisCacheProvider()
        {
            Caches = new ConcurrentDictionary<string, Lazy<ICache>>();
        }


        public RedisCacheProvider(RedisConfig config = null)
        {
            _config = config;
        }

        /// <summary>
        /// 获取 缓存是否可用
        /// </summary>
        public bool Enabled { get; set; }

        /// <inheritdoc />
        /// <summary> 获取缓存对象 </summary>
        /// <param name="regionName">缓存区域名称</param>
        /// <returns></returns>
        public ICache GetCache(string regionName)
        {
            var lazyCache = Caches.GetOrAdd(regionName,
                k => new Lazy<ICache>(() => _config != null ? new RedisCache(k, _config) : new RedisCache(k)));
            return lazyCache.Value;
        }
    }
}
