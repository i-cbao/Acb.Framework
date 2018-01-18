using System.Collections.Generic;

namespace Acb.Core
{
    public interface IPagedList<TEntity> : IList<TEntity>
    {
        int Index { get; }
        int Size { get; }
        int Total { get; }
        int Pages { get; }
        bool HasPrev { get; }
        bool HasNext { get; }
        PagedList<T> ConvertData<T>(IEnumerable<T> enumerable);
    }
}
