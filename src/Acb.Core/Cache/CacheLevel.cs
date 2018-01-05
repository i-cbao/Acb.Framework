using System;

namespace Acb.Core.Cache
{
    /// <summary> 缓存级别 </summary>
    [Flags]
    public enum CacheLevel
    {
        /// <summary> 一级缓存，内存 </summary>
        First = 1,

        /// <summary> 二级缓存，分布式 </summary>
        Second = 2,

        /// <summary> 两级缓存 </summary>
        Both = 3
    }
}
