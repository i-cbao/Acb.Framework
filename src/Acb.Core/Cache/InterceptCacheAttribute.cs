using System;

namespace Acb.Core.Cache
{
    public enum CacheMethod
    {
        /// <summary> 获取缓存 </summary>
        Get,
        /// <summary> 设置缓存 </summary>
        Put,
        /// <summary> 删除缓存 </summary>
        Remove
    }

    /// <summary> 缓存拦截 </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class InterceptCacheAttribute : Attribute
    {
        /// <summary> 缓存方法 </summary>
        public CacheMethod Method { get; }

        /// <summary> 要删除的键 </summary>
        public string[] RemoveKeys { get; set; }

        /// <summary> 缓存前缀 </summary>
        public string Prefix { get; set; }
        /// <summary> 缓存建 </summary>
        public string Key { get; set; }

        /// <summary> 缓存时间(秒) </summary>
        public int Time { get; set; }
        /// <summary> 缓存时间(天) </summary>
        public int Date { get; set; }

        /// <summary> 构造函数 </summary>
        /// <param name="method"></param>
        public InterceptCacheAttribute(CacheMethod method = CacheMethod.Get)
        {
            Method = method;
        }

        /// <summary> 构造函数 </summary>
        /// <param name="method"></param>
        /// <param name="names"></param>
        public InterceptCacheAttribute(CacheMethod method, params string[] names) : this(method)
        {
            RemoveKeys = names;
        }
    }
}
