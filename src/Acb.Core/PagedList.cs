using System.Collections.Generic;
using System.Linq;

namespace Acb.Core
{
    /// <summary> 数据页 </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PagedList<TEntity> : List<TEntity>, IPagedList<TEntity>
    {
        public PagedList() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex">页索引（从1开始）</param>
        /// <param name="pageCount"></param>
        public PagedList(IQueryable<TEntity> queryable, int pageIndex, int pageCount)
        {
            var total = queryable.Count();
            TotalCount = total;
            TotalPages = total / pageCount;

            if (total % pageCount > 0)
                TotalPages++;

            PageSize = pageCount;
            PageIndex = pageIndex;
            AddRange(queryable.Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList());
        }

        /// <summary> 数据分页 </summary>
        /// <param name="list"></param>
        /// <param name="pageIndex">页索引（从1开始）</param>
        /// <param name="pageCount"></param>
        public PagedList(IList<TEntity> list, int pageIndex, int pageCount)
        {
            TotalCount = list.Count();
            TotalPages = TotalCount / pageCount;

            if (TotalCount % pageCount > 0)
                TotalPages++;

            PageSize = pageCount;
            PageIndex = pageIndex;
            AddRange(list.Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList());
        }

        /// <summary> 初始化数据 </summary>
        /// <param name="enumerable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="totalCount"></param>
        public PagedList(IEnumerable<TEntity> enumerable, int pageIndex, int pageCount, int totalCount)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageCount;

            if (TotalCount % pageCount > 0)
                TotalPages++;

            PageSize = pageCount;
            PageIndex = pageIndex;
            AddRange(enumerable);
        }

        /// <summary> 转换数据 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public PagedList<T> ConvertData<T>(IEnumerable<T> enumerable)
        {
            return new PagedList<T>(enumerable, PageIndex, PageSize, TotalCount);
        }

        /// <summary> 页码(从1开始) </summary>
        public int PageIndex { set; get; }
        /// <summary> 每页数量 </summary>
        public int PageSize { set; get; }
        /// <summary> 总数量 </summary>
        public int TotalCount { set; get; }
        /// <summary> 总页数 </summary>
        public int TotalPages { set; get; }

        /// <summary> 是否有上一页 </summary>
        public bool HasPrev => PageIndex > 1;

        /// <summary> 是否有下一页 </summary>
        public bool HasNext => PageIndex < TotalPages;
    }
}
