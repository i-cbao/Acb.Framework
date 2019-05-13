using Acb.Core.Logging;
using Acb.Core.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.Cache
{
    /// <summary> 缓存执行者 </summary>
    internal sealed class InternalCacher : ICache
    {
        private static readonly ILogger Logger = LogManager.Logger<InternalCacher>();

        private readonly Dictionary<CacheLevel, ICache> _caches;

        /// <summary> 本地缓存时间 </summary>
        private readonly double _memoryExpireMinutes;

        /// <summary> 初始化一个<see cref="InternalCacher"/>类型的新实例 </summary>
        public InternalCacher(string region, CacheLevel level, double expireMinutes)
        {
            _caches = CacheManager.Providers.Where(m => m.Value != null && (m.Key & level) > 0)
                .ToDictionary(k => k.Key, v => v.Value.Value.GetCache(region));
            if (_caches.Count == 0)
            {
                Logger.Warn("no cache provider！");
            }
            _memoryExpireMinutes = expireMinutes;
        }

        private TimeSpan MemoryExpired(TimeSpan expired)
        {
            var memory = TimeSpan.FromMinutes(_memoryExpireMinutes);
            return expired < memory ? expired : memory;
        }

        private DateTime MemoryExpired(DateTime expired)
        {
            var memory = Clock.Now.AddMinutes(_memoryExpireMinutes);
            return expired < memory ? expired : memory;
        }

        public void Set(string key, object value)
        {
            foreach (var cache in _caches)
            {
                if (cache.Key == CacheLevel.First && _caches.ContainsKey(CacheLevel.Second))
                    cache.Value.Set(key, value, Clock.Now.AddMinutes(_memoryExpireMinutes));
                else
                    cache.Value.Set(key, value);
            }
        }

        public void Set(string key, object value, TimeSpan expire)
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

        public void Set(string key, object value, DateTime expire)
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

        public object Get(string key)
        {
            object value;
            if (_caches.TryGetValue(CacheLevel.First, out var cache))
            {
                //先从一级缓存读取
                value = cache.Get(key);
                if (value != null)
                    return value;
            }
            if (!_caches.TryGetValue(CacheLevel.Second, out cache))
                return null;
            value = cache.Get(key);
            if (value == null || !_caches.TryGetValue(CacheLevel.First, out cache))
                return value;
            //设置一级缓存
            cache.Set(key, value, Clock.Now.AddMinutes(_memoryExpireMinutes));
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

        public T Get<T>(string key)
        {
            var value = default(T);
            if (_caches.TryGetValue(CacheLevel.First, out var cache))
            {
                //先从一级缓存读取
                value = cache.Get<T>(key);
                if (value != null)
                    return value;
            }
            if (!_caches.TryGetValue(CacheLevel.Second, out cache))
                return value;
            value = cache.Get<T>(key);
            if (value == null || !_caches.TryGetValue(CacheLevel.First, out cache))
                return value;
            //设置一级缓存
            cache.Set(key, value, Clock.Now.AddMinutes(_memoryExpireMinutes));
            return value;
        }

        public void Remove(string key)
        {
            foreach (var cache in _caches.Values)
            {
                cache.Remove(key);
            }
        }

        public void Remove(IEnumerable<string> keys)
        {
            var enumerable = keys as string[] ?? keys.ToArray();
            foreach (var cache in _caches.Values)
            {
                cache.Remove(enumerable);
            }
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

        public void ExpireEntryAt(string key, DateTime dateTime)
        {
            var memoryExpired = MemoryExpired(dateTime);
            foreach (var cache in _caches)
            {
                if (cache.Key == CacheLevel.First && _caches.ContainsKey(CacheLevel.Second))
                    cache.Value.ExpireEntryAt(key, memoryExpired);
                else
                    cache.Value.ExpireEntryAt(key, dateTime);
            }
        }
    }
}
