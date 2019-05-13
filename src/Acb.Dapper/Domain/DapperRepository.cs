using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Domain.Entities;
using System.Collections.Generic;

namespace Acb.Dapper.Domain
{
    /// <summary> 基础仓储 </summary>
    /// <typeparam name="T"></typeparam>
    public partial class DapperRepository<T> : DRepository
        where T : IEntity
    {
        public DapperRepository() : base(CurrentIocManager.Resolve<IUnitOfWork>()) { }

        /// <summary> 构造 </summary>
        /// <param name="unitOfWork"></param>
        public DapperRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary> 查询所有数据 </summary>
        public IEnumerable<T> Query()
        {
            return Connection.QueryAll<T>();
        }

        /// <summary> 根据主键查询单条 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public T QueryById(object key, string keyColumn = null)
        {
            return Connection.QueryById<T>(key, keyColumn);
        }

        /// <summary> 插入单条数据,不支持有自增列 </summary>
        /// <param name="model"></param>
        /// <param name="excepts">过滤项(如：自增ID)</param>
        /// <returns></returns>
        public int Insert(T model, string[] excepts = null)
        {

            return TransConnection.Insert(model, excepts, Trans);
        }

        /// <summary> 批量插入 </summary>
        /// <param name="models"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        public int Insert(IEnumerable<T> models, string[] excepts = null)
        {
            return TransConnection.Insert<T>(models, excepts, Trans);
        }

        /// <summary> 删除 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public int Delete(object key, string keyColumn = null)
        {
            return TransConnection.Delete<T>(key, keyColumn, Trans);
        }
    }
}
