using System.Collections.Generic;

namespace Acb.Core
{
    /// <summary> 分页接口类 </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IPagedList<TEntity> : IList<TEntity>
    {
        /// <summary> 页码(从1开始) </summary>
        int Index { get; }
        /// <summary> 每页显示数 </summary>
        int Size { get; }
        /// <summary> 总数 </summary>
        int Total { get; }
        /// <summary> 总页数 </summary>
        int Pages { get; }
        /// <summary> 是否有上一页 </summary>
        bool HasPrev { get; }
        /// <summary> 是否有下一页 </summary>
        bool HasNext { get; }
        /// <summary> 数据转换 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        PagedList<T> ConvertData<T>(IEnumerable<T> enumerable);
    }
}
