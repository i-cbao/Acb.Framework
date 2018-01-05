using System;
using System.Collections.Generic;

namespace Acb.Core.Cache
{
    /// <summary> 缓存接口 </summary>
    public interface ICache
    {
        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set(string key, object value);

        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        void Set(string key, object value, TimeSpan expire);

        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        void Set(string key, object value, DateTime expire);

        /// <summary> 获取缓存 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary> 获取所有缓存 </summary>
        /// <returns></returns>
        IEnumerable<object> GetAll();

        /// <summary> 获取缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary> 移除缓存 </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary> 批量异常缓存 </summary>
        /// <param name="keys"></param>
        void Remove(IEnumerable<string> keys);
        /// <summary> 设置过期时间 </summary>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        void ExpireEntryIn(string key, TimeSpan timeSpan);

        /// <summary> 设置过期时间 </summary>
        /// <param name="key"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        void ExpireEntryAt(string key, DateTime dateTime);

        /// <summary> 清空缓存 </summary>
        void Clear();
    }
}
