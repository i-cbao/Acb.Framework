using System;
using System.Collections.Generic;

namespace Acb.Core.Cache
{
    /// <summary> 缓存基类 </summary>
    public abstract class BaseCache : ICache
    {
        /// <summary> 区域 </summary>
        public abstract string Region { get; }

        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Set(string key, object value);

        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        public abstract void Set(string key, object value, TimeSpan expire);

        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        public abstract void Set(string key, object value, DateTime expire);

        /// <summary> 获取缓存 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract object Get(string key);

        /// <summary> 获取所有缓存 </summary>
        public abstract IEnumerable<object> GetAll();

        /// <summary> 获取缓存 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract T Get<T>(string key);

        /// <summary> 移除缓存 </summary>
        /// <param name="key"></param>
        public abstract void Remove(string key);

        /// <summary> 移除缓存 </summary>
        /// <param name="key"></param>
        public abstract void Remove(IEnumerable<string> key);

        /// <summary> 清空协同 </summary>
        public abstract void Clear();

        public abstract void ExpireEntryIn(string key, TimeSpan timeSpan);

        public abstract void ExpireEntryAt(string key, DateTime dateTime);

        /// <summary> 本地缓存时间 </summary>
        /// <param name="expire"></param>
        public virtual void MemoryExpire(TimeSpan expire)
        {
        }

        /// <summary> 获取缓存键 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string GetKey(string key)
        {
            return string.Concat(Region, ":", key); //, "@", key.GetHashCode()
        }
    }
}
