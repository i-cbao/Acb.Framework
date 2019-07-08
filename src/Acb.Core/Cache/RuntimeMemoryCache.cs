using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Acb.Core.Cache
{
    /// <summary> 运行时缓存 </summary>
    public class RuntimeMemoryCache : BaseCache
    {
        private IMemoryCache _cache;

        public RuntimeMemoryCache(string region) : base(region)
        {
            _cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        public override void Set(string key, object value, TimeSpan? expire = null)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            if (expire.HasValue)
                _cache.Set(cacheKey, new DictionaryEntry(key, value), expire.Value);
            else
                _cache.Set(cacheKey, new DictionaryEntry(key, value));
        }

        public override object Get(string key, Type type)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var cacheKey = GetKey(key);
            var value = _cache.Get(cacheKey);
            if (value == null || !(value is DictionaryEntry entry))
                return null;
            return key.Equals(entry.Key) ? entry.Value : null;
        }

        public override IEnumerable<object> GetAll()
        {
            throw new NotImplementedException();
        }

        public override void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            var cacheKey = GetKey(key);
            _cache.Remove(cacheKey);
        }

        public override TimeSpan? ExpireTime(string key)
        {
            return null;
        }

        public override void Clear()
        {
            _cache.Dispose();
            _cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        public override void ExpireEntryIn(string key, TimeSpan timeSpan)
        {
            var value = Get(key, null);
            if (value != null)
                Set(key, value, timeSpan);
        }
    }
}
