using Acb.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.Cache
{
    /// <summary> 缓存执行者 </summary>
    internal sealed class InternalCacher : ICache
    {
        private readonly ILogger _logger;

        private readonly Dictionary<CacheLevel, ICache> _caches;

        /// <summary> 本地缓存时间 </summary>
        private readonly double _memoryExpireMinutes;

        /// <summary> 初始化一个<see cref="InternalCacher"/>类型的新实例 </summary>
        public InternalCacher(string region, CacheLevel level, double expireMinutes)
        {
            Region = region;
            _logger = LogManager.Logger<InternalCacher>();
            _caches = CacheManager.Providers.Where(m => m.Value != null && (m.Key & level) > 0)
                .ToDictionary(k => k.Key, v => v.Value.Value.GetCache(region));
            if (_caches.Count == 0)
            {
                _logger.Warn("no cache provider！");
            }
            _memoryExpireMinutes = expireMinutes;
        }

        private TimeSpan MemoryExpired(TimeSpan? expired)
        {
            var memory = TimeSpan.FromMinutes(_memoryExpireMinutes);
            if (!expired.HasValue) return memory;
            return expired.Value < memory ? expired.Value : memory;
        }

        public string Region { get; }

        public void Set(string key, object value, TimeSpan? expire = null)
        {
            var memoryExpired = MemoryExpired(expire);
            foreach (var cache in _caches)
            {
                if (cache.Key == CacheLevel.First && _caches.ContainsKey(CacheLevel.Second))
                    cache.Value.Set(key, value, memoryExpired);
                else
                    cache.Value.Set(key, value, expire);
            }
        }

        public object Get(string key, Type type)
        {
            object value;
            if (_caches.TryGetValue(CacheLevel.First, out var cache))
            {
                //先从一级缓存读取
                value = cache.Get(key, type);
                if (value != null)
                {
                    _logger.Debug("get from first cache");
                    return value;
                }
            }
            if (!_caches.TryGetValue(CacheLevel.Second, out cache))
                return null;
            value = cache.Get(key, type);
            _logger.Debug("get from second cache");
            //没有值 或 没有一级缓存 直接返回
            if (value == null || !_caches.TryGetValue(CacheLevel.First, out cache))
                return value;
            //设置一级缓存
            var memoryTime = TimeSpan.FromMinutes(_memoryExpireMinutes);
            var time = cache.ExpireTime(key);
            if (time.HasValue && time.Value < memoryTime)
                memoryTime = time.Value;
            cache.Set(key, value, memoryTime);
            return value;
        }

        public IEnumerable<object> GetAll()
        {
            var values = new List<object>();
            foreach (var cache in _caches.Values)
            {
                values = cache.GetAll().ToList();
                if (values.Count != 0)
                {
                    break;
                }
            }
            return values;
        }

        public void Remove(string key)
        {
            foreach (var cache in _caches.Values)
            {
                cache.Remove(key);
            }
        }

        public TimeSpan? ExpireTime(string key)
        {
            foreach (var cache in _caches.Values)
            {
                var time = cache.ExpireTime(key);
                if (time.HasValue) return time;
            }

            return null;
        }

        public void Clear()
        {
            foreach (var cache in _caches.Values)
            {
                cache.Clear();
            }
        }

        public void ExpireEntryIn(string key, TimeSpan timeSpan)
        {
            var memoryExpired = MemoryExpired(timeSpan);
            foreach (var cache in _caches)
            {
                if (cache.Key == CacheLevel.First && _caches.ContainsKey(CacheLevel.Second))
                    cache.Value.ExpireEntryIn(key, memoryExpired);
                else
                    cache.Value.ExpireEntryIn(key, timeSpan);
            }
        }
    }
}
