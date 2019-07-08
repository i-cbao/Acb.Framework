using Acb.Core.Extensions;
using Acb.Core.Timing;
using System;
using System.Collections.Generic;

namespace Acb.Core.Cache
{
    /// <summary> 缓存接口 </summary>
    public interface ICache
    {
        string Region { get; }
        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        void Set(string key, object value, TimeSpan? expire = null);

        /// <summary> 获取缓存 </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Get(string key, Type type);

        /// <summary> 获取所有缓存 </summary>
        /// <returns></returns>
        IEnumerable<object> GetAll();

        /// <summary> 移除缓存 </summary>
        /// <param name="key"></param>
        void Remove(string key);
        /// <summary> 设置过期时间 </summary>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        void ExpireEntryIn(string key, TimeSpan timeSpan);

        TimeSpan? ExpireTime(string key);

        /// <summary> 清空缓存 </summary>
        void Clear();
    }

    public static class CacheExtensions
    {
        /// <summary> 设置缓存 </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireAt"></param>
        public static void Set(this ICache cache, string key, object value, DateTime expireAt)
        {
            cache.Set(key, value, expireAt - Clock.Now);
        }

        /// <summary> 获取缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ICache cache, string key)
        {
            var value = cache.Get(key, typeof(T));
            return value == null ? default(T) : value.CastTo<T>();
        }

        /// <summary> 设置过期时间 </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static void ExpireEntryAt(this ICache cache, string key, DateTime dateTime)
        {
            cache.ExpireEntryIn(key, dateTime - Clock.Now);
        }

        /// <summary> 移除缓存 </summary>
        /// <param name="cache"></param>
        /// <param name="keys"></param>
        public static void Remove(this ICache cache, IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                    continue;
                cache.Remove(key);
            }
        }
    }
}
