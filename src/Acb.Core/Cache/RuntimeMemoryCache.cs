using Acb.Core.Extensions;
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
        private readonly string _region;
        private IMemoryCache _cache;

        public RuntimeMemoryCache(string region)
        {
            _region = region;
            _cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }
        /// <summary> 缓存区域 </summary>
        public override string Region => _region;

        public override void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Set(cacheKey, new DictionaryEntry(key, value));
        }

        public override void Set(string key, object value, TimeSpan expire)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Set(cacheKey, new DictionaryEntry(key, value), expire);
        }

        public override void Set(string key, object value, DateTime expire)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Set(cacheKey, new DictionaryEntry(key, value), expire);
        }

        public override object Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var cacheKey = GetKey(key);
            var value = _cache.Get(cacheKey);
            if (value == null)
                return null;
            var entry = (DictionaryEntry)value;
            return key.Equals(entry.Key) ? entry.Value : null;
        }

        public override IEnumerable<object> GetAll()
        {
            throw new NotImplementedException();
        }

        public override T Get<T>(string key)
        {
            var value = Get(key);
            return value == null ? default(T) : value.CastTo<T>();
        }

        public override void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            var cacheKey = GetKey(key);
            _cache.Remove(cacheKey);
        }

        public override void Remove(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public override void Clear()
        {
            _cache.Dispose();
            _cache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        public override void ExpireEntryIn(string key, TimeSpan timeSpan)
        {
            var value = Get(key);
            if (value != null)
                Set(key, value, timeSpan);
        }

        public override void ExpireEntryAt(string key, DateTime dateTime)
        {
            var value = Get(key);
            if (value != null)
                Set(key, value, dateTime);
        }
    }
}
