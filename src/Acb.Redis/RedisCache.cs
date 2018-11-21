using Acb.Core.Cache;
using Acb.Core.Timing;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Redis
{
    /// <summary> 缓存实现类 </summary>
    public class RedisCache : BaseCache
    {
        private readonly string _region;
        private readonly string _configName;
        private readonly RedisConfig _config;
        private readonly RedisManager _redisManager;
        public RedisCache(string region, string configName = null)
        {
            _region = region;
            _configName = configName;
            _redisManager = RedisManager.Instance;
        }

        public RedisCache(string region, RedisConfig config)
        {
            _region = region;
            _config = config;
            _redisManager = RedisManager.Instance;
        }

        public override string Region => _region;

        private IDatabase GetDatabase()
        {
            return _config != null ? _redisManager.GetDatabase(_config) : _redisManager.GetDatabase(_configName);
        }

        private IServer GetServer()
        {
            return _config != null ? _redisManager.GetServer(_config) : _redisManager.GetServer(_configName);
        }

        private void SetValue(string key, object value, TimeSpan? expired = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            key = GetKey(key);
            var db = GetDatabase();
            if (value == null)
            {
                db.KeyDelete(key);
                return;
            }
            db.Set(key, value, expired);
        }
        private T GetValue<T>(string key, bool format = true)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T);
            if (format)
                key = GetKey(key);
            var db = GetDatabase();
            return db.Get<T>(key);
        }

        public override void Set(string key, object value)
        {
            SetValue(key, value);
        }

        public override void Set(string key, object value, TimeSpan expire)
        {
            SetValue(key, value, expire);
        }

        public override void Set(string key, object value, DateTime expire)
        {
            SetValue(key, value, expire - Clock.Now);
        }



        public override object Get(string key)
        {
            return GetValue<object>(key);
        }

        public override IEnumerable<object> GetAll()
        {
            var token = string.Concat(_region, ":*");
            var server = GetServer();
            var keys = server.Keys(pattern: token);
            var list = new List<object>();
            foreach (var key in keys)
            {
                var item = GetValue<object>(key, false);
                list.Add(item);
            }
            return list;
        }

        public override T Get<T>(string key)
        {
            return GetValue<T>(key);
        }

        public override void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            var cacheKey = GetKey(key);
            var db = GetDatabase();
            db.KeyDelete(cacheKey);
        }

        public override void Remove(IEnumerable<string> keys)
        {
            var cacheKeys = keys.Select(GetKey);
            var db = GetDatabase();
            foreach (var key in cacheKeys)
            {
                db.KeyDelete(key);
            }
        }

        public override void Clear()
        {
            var token = $"{_region}:*";
            var server = GetServer();
            var keys = server.Keys(pattern: token).Select(t => t.ToString());
            Remove(keys);
        }

        public override void ExpireEntryIn(string key, TimeSpan timeSpan)
        {
            var cacheKey = GetKey(key);
            var db = GetDatabase();
            db.KeyExpire(cacheKey, timeSpan);
        }

        public override void ExpireEntryAt(string key, DateTime dateTime)
        {
            ExpireEntryIn(key, dateTime - Clock.Now);
        }
    }
}
