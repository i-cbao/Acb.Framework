using Acb.Core.Extensions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Acb.Redis
{
    /// <summary> Redis扩展 </summary>
    public static class RedisExtensions
    {
        #region 私有方法

        /// <summary> 序列化对象 </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static RedisValue Serialize(object obj)
        {
            if (obj == null)
            {
                return RedisValue.Null;
            }
            var type = obj.GetType();
            return type.IsSimpleType() ? obj.ToString() : JsonConvert.SerializeObject(obj);
        }

        /// <summary> 反序列化对象 </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object Deserialize(RedisValue value, Type type)
        {
            if (!value.HasValue)
            {
                return null;
            }

            if (typeof(string) == type)
                return value.ToString();
            return type.IsSimpleType() ? value.ToString().CastTo(type) : JsonConvert.DeserializeObject(value, type);
        }

        private static T Deserialize<T>(RedisValue value)
        {
            var obj = Deserialize(value, typeof(T));
            return obj == null ? default(T) : (T)obj;
        }

        #endregion

        /// <summary> 获取缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static T Get<T>(this IDatabase database, string key)
        {
            var value = database.StringGet(key);
            return Deserialize<T>(value);
        }

        /// <summary> 获取缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDatabase database, string key)
        {
            var value = await database.StringGetAsync(key);
            return Deserialize<T>(value);
        }

        /// <summary> 获取缓存 </summary>
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Get(this IDatabase database, string key, Type type)
        {
            var val = database.StringGet(key);
            return Deserialize(val, type);
        }

        /// <summary> 获取缓存 </summary>
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static async Task<object> GetAsync(this IDatabase database, string key)
        {
            return await database.GetAsync<object>(key);
        }

        /// <summary> 设置缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expired">过期时间</param>
        public static void Set<T>(this IDatabase database, string key, T value, TimeSpan? expired = null)
        {
            database.StringSet(key, Serialize(value), expired);
        }

        /// <summary> 设置缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expired">过期时间</param>
        public static async Task SetAsync<T>(this IDatabase database, string key, T value, TimeSpan? expired = null)
        {
            await database.StringSetAsync(key, Serialize(value), expired);
        }
    }
}
