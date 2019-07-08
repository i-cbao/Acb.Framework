using Acb.Core.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Redis
{
    /// <summary> 缓存实现类 </summary>
    public class RedisCache : BaseCache
    {
        private readonly string _configName;
        private readonly RedisConfig _config;
        private readonly RedisManager _redisManager;
        public RedisCache(string region, string configName = null) : base(region)
        {
            _configName = configName;
            _redisManager = RedisManager.Instance;
        }

        public RedisCache(string region, RedisConfig config) : base(region)
        {
            _config = config;
            _redisManager = RedisManager.Instance;
        }

        private IDatabase GetDatabase()
        {
            return _config != null ? _redisManager.GetDatabase(_config) : _redisManager.GetDatabase(_configName);
        }

        private IServer GetServer()
        {
            return _config != null ? _redisManager.GetServer(_config) : _redisManager.GetServer(_configName);
        }

        private object GetValue(string key, Type type)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var db = GetDatabase();
            return db.Get(key, type);
        }

        public override void Set(string key, object value, TimeSpan? expired = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            var cacheKey = GetKey(key);
            var db = GetDatabase();
            if (value == null)
            {
                db.KeyDelete(cacheKey);
            }
            else
            {
                db.Set(cacheKey, value, expired);
            }
            _redisManager.CachePublish(Region, key);
        }

        public override object Get(string key, Type type)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var cacheKey = GetKey(key);
            return GetValue(cacheKey, type);
        }

        public override IEnumerable<object> GetAll()
        {
            var token = string.Concat(Region, ":*");
            var server = GetServer();
            var keys = server.Keys(pattern: token);
            return keys.Select(key => GetValue(key, typeof(object))).ToList();
        }

        public override void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            var cacheKey = GetKey(key);
            var db = GetDatabase();
            var result = db.KeyDelete(cacheKey);
            if (result)
                _redisManager.CachePublish(Region, key);
        }

        public override TimeSpan? ExpireTime(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var cacheKey = GetKey(key);
            var db = GetDatabase();
            var t = db.StringGetWithExpiry(cacheKey);
            return t.Expiry;
        }

        public override void Clear()
        {
            var token = $"{Region}:*";
            var server = GetServer();
            var keys = server.Keys(pattern: token).Select(t => t.ToString()).ToArray();
            this.Remove(keys);
            foreach (var key in keys)
            {
                _redisManager.CachePublish(Region, key.Replace($"^{Region}:", string.Empty));
            }
        }

        public override void ExpireEntryIn(string key, TimeSpan timeSpan)
        {
            var cacheKey = GetKey(key);
            var db = GetDatabase();
            var result = db.KeyExpire(cacheKey, timeSpan);
            if (result)
                _redisManager.CachePublish(Region, key);
        }
    }
}
