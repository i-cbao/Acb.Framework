using Acb.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Acb.Dapper.Domain
{
    /// <summary> 基础仓储 </summary>
    /// <typeparam name="T"></typeparam>
    public class DapperRepository<T> : DRepository
        where T : IEntity
    {
        /// <summary> 构造 </summary>
        /// <param name="connectionName"></param>
        public DapperRepository(string connectionName) : base(connectionName)
        {
        }

        /// <summary> 构造 </summary>
        /// <param name="enumType"></param>
        public DapperRepository(Enum enumType) : this(enumType.ToString())
        {
        }

        /// <summary> 查询所有数据 </summary>
        public IEnumerable<T> Query()
        {
            return Connection.Query<T>();
        }

        /// <summary> 根据主键查询单条 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public T QueryById(object key, string keyColumn = "id")
        {
            return Connection.QueryById<T>(key, keyColumn);
        }

        /// <summary> 插入单条数据,不支持有自增列 </summary>
        /// <param name="model"></param>
        /// <param name="excepts">过滤项(如：自增ID)</param>
        /// <returns></returns>
        public int Insert(T model, string[] excepts = null)
        {
            return Connection.Insert(model, excepts);
        }

        /// <summary> 批量插入 </summary>
        /// <param name="models"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        public int BatchInsert(IEnumerable<T> models, string[] excepts = null)
        {
            return Connection.BatchInsert(models, excepts);
        }

        /// <summary> 删除 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public int Delete(object key, string keyColumn = "id")
        {
            return Connection.Delete<T>(key, keyColumn);
        }
    }
}
