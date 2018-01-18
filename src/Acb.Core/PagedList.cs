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
        /// <param name="index">页索引（从1开始）</param>
        /// <param name="size"></param>
        public PagedList(IQueryable<TEntity> queryable, int index, int size)
        {
            var total = queryable.Count();
            Total = total;
            Pages = total / size;

            if (total % size > 0)
                Pages++;

            Size = size;
            Index = index;
            AddRange(queryable.Skip((index - 1) * size).Take(size).ToList());
        }

        /// <summary> 数据分页 </summary>
        /// <param name="list"></param>
        /// <param name="index">页索引（从1开始）</param>
        /// <param name="size"></param>
        public PagedList(IList<TEntity> list, int index, int size)
        {
            Total = list.Count();
            Pages = Total / size;

            if (Total % size > 0)
                Pages++;

            Size = size;
            Index = index;
            AddRange(list.Skip((index - 1) * size).Take(size).ToList());
        }

        /// <summary> 初始化数据 </summary>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        public PagedList(IEnumerable<TEntity> enumerable, int index, int size, int total)
        {
            Total = total;
            Pages = Total / size;

            if (Total % size > 0)
                Pages++;

            Size = size;
            Index = index;
            AddRange(enumerable);
        }

        /// <summary> 转换数据 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public PagedList<T> ConvertData<T>(IEnumerable<T> enumerable)
        {
            return new PagedList<T>(enumerable, Index, Size, Total);
        }

        /// <summary> 页码(从1开始) </summary>
        public int Index { set; get; }
        /// <summary> 每页数量 </summary>
        public int Size { set; get; }
        /// <summary> 总数量 </summary>
        public int Total { set; get; }
        /// <summary> 总页数 </summary>
        public int Pages { set; get; }

        /// <summary> 是否有上一页 </summary>
        public bool HasPrev => Index > 1;

        /// <summary> 是否有下一页 </summary>
        public bool HasNext => Index < Pages;
    }
}
