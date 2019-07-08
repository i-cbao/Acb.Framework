using System;
using System.Collections.Generic;

namespace Acb.Core.Cache
{
    /// <inheritdoc />
    /// <summary> 缓存基类 </summary>
    public abstract class BaseCache : ICache
    {
        protected BaseCache(string region)
        {
            Region = region;
        }

        /// <summary> 区域 </summary>
        public string Region { get; }

        /// <inheritdoc />
        /// <summary> 设置缓存 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        public abstract void Set(string key, object value, TimeSpan? expire = null);

        /// <summary> 获取缓存 </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract object Get(string key, Type type);

        /// <summary> 获取所有缓存 </summary>
        public abstract IEnumerable<object> GetAll();

        /// <summary> 移除缓存 </summary>
        /// <param name="key"></param>
        public abstract void Remove(string key);

        public abstract TimeSpan? ExpireTime(string key);

        /// <summary> 清空协同 </summary>
        public abstract void Clear();

        public abstract void ExpireEntryIn(string key, TimeSpan timeSpan);

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
            return string.IsNullOrWhiteSpace(key) ? key : string.Concat(Region, ":", key);
        }
    }
}
