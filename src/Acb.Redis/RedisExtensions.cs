using Acb.Core.Extensions;
using Acb.Core.Serialize;
using StackExchange.Redis;
using System;

namespace Acb.Redis
{
    /// <summary> Redis扩展 </summary>
    public static class RedisExtensions
    {
        #region 私有方法
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static RedisValue Serialize(object obj)
        {
            if (obj == null)
            {
                return RedisValue.Null;
            }
            var type = obj.GetType();
            return type == typeof(string) || type.IsValueType ? obj.ToString() : JsonHelper.ToJson(obj);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T Deserialize<T>(RedisValue value)
        {
            if (!value.HasValue)
            {
                return default(T);
            }
            var type = typeof(T);
            return type == typeof(string) || type.IsValueType ? value.ToString().CastTo<T>() : JsonHelper.Json<T>(value);
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
        /// <param name="database"></param>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static object Get(this IDatabase database, string key)
        {
            return database.Get<object>(key);
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
    }
}
