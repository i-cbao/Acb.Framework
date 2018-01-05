using System.Collections.Generic;

namespace Acb.Core
{
    public interface IPagedList<TEntity> : IList<TEntity>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPrev { get; }
        bool HasNext { get; }
        PagedList<T> ConvertData<T>(IEnumerable<T> enumerable);
    }
}
