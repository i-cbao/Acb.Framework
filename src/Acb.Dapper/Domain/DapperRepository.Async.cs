using Acb.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Dapper.Domain
{
    public partial class DapperRepository<T> where T : IEntity
    {
        /// <summary> 查询所有数据 </summary>
        public Task<IEnumerable<T>> QueryAsync()
        {
            return Connection.QueryAllAsync<T>();
        }

        /// <summary> 根据主键查询单条 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public Task<T> QueryByIdAsync(object key, string keyColumn = null)
        {
            return Connection.QueryByIdAsync<T>(key, keyColumn);
        }

        /// <summary> 插入单条数据,不支持有自增列 </summary>
        /// <param name="model"></param>
        /// <param name="excepts">过滤项(如：自增ID)</param>
        /// <returns></returns>
        public Task<int> InsertAsync(T model, string[] excepts = null)
        {
            return Connection.InsertAsync(model, excepts, Trans);
        }

        /// <summary> 批量插入 </summary>
        /// <param name="models"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(IEnumerable<T> models, string[] excepts = null)
        {
            return Connection.InsertAsync(models, excepts, Trans);
        }

        /// <summary> 删除 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(object key, string keyColumn = null)
        {
            return Connection.DeleteAsync<T>(key, keyColumn, Trans);
        }
    }
}
