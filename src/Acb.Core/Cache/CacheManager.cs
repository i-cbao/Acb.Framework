using System;
using System.Collections.Concurrent;

namespace Acb.Core.Cache
{
    /// <summary> 缓存管理器 </summary>
    public static class CacheManager
    {
        internal static readonly ConcurrentDictionary<CacheLevel, Lazy<ICacheProvider>> Providers;
        /// <summary> 区域缓存 </summary>
        private static readonly ConcurrentDictionary<string, Lazy<ICache>> Cachers;

        static CacheManager()
        {
            //2级缓存
            Providers = new ConcurrentDictionary<CacheLevel, Lazy<ICacheProvider>>();
            Cachers = new ConcurrentDictionary<string, Lazy<ICache>>();
        }

        /// <summary> 设置提供者 </summary>
        /// <param name="provider"></param>
        /// <param name="level"></param>
        public static void SetProvider(CacheLevel level, ICacheProvider provider)
        {
            Providers.TryAdd(level, new Lazy<ICacheProvider>(() => provider));
        }

        /// <summary> 移除提供者 </summary>
        /// <param name="level"></param>
        public static void RemoveProvider(CacheLevel level)
        {
            Providers.TryRemove(level, out _);
        }

        /// <summary> 获取指定区域的缓存执行者实例 </summary>
        /// <param name="region">缓存区域</param>
        /// <param name="level">默认：一级缓存</param>
        /// <param name="memeryExpiredMinutes">一级缓存时间(分,仅当使用两级缓存时生效)</param>
        /// <returns></returns>
        public static ICache GetCacher(string region, CacheLevel level = CacheLevel.First,
            double memeryExpiredMinutes = 5)
        {
            var cacheLazy = Cachers.GetOrAdd(region,
                key => new Lazy<ICache>(() => new InternalCacher(key, level, memeryExpiredMinutes)));
            return cacheLazy.Value;
        }

        /// <summary>
        /// 获取指定类型的缓存执行者实例
        /// </summary>
        /// <param name="type">类型实例</param>
        /// <param name="level">默认：一级缓存</param>
        /// <param name="memeryExpiredMinutes">一级缓存时间(分,仅当使用两级缓存时生效)</param>
        public static ICache GetCacher(Type type, CacheLevel level = CacheLevel.First, double memeryExpiredMinutes = 5)
        {
            return GetCacher(type.FullName, level, memeryExpiredMinutes);
        }

        /// <summary> 获取指定类型的缓存执行者实例 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level">默认：一级缓存</param>
        /// <param name="memeryExpiredMinutes">一级缓存时间(分,仅当使用两级缓存时生效)</param>
        /// <returns></returns>
        public static ICache GetCacher<T>(CacheLevel level = CacheLevel.First, double memeryExpiredMinutes = 5)
        {
            return GetCacher(typeof(T), level, memeryExpiredMinutes);
        }
    }
}
