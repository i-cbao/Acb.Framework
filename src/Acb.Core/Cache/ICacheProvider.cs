namespace Acb.Core.Cache
{
    /// <summary> 缓存提供者约定 </summary>
    public interface ICacheProvider
    {
        /// <summary> 获取缓存对象 </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        ICache GetCache(string regionName);
    }
}
