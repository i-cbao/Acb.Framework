using Acb.Core;
using Acb.Core.Dependency;
using System;
using System.Data;

namespace Acb.Dapper.Domain
{
    public abstract class DRepository : IScopedDependency
    {
        private readonly string _defaultConnectionName;
        private readonly ConnectionFactory _factory;
        /// <summary> 获取默认连接 </summary>
        protected IDbConnection Connection => GetConnection(_defaultConnectionName);

        protected DRepository(string connectionName = null)
        {
            _factory = CurrentIocManager.Resolve<ConnectionFactory>();
            _defaultConnectionName = connectionName;
        }

        protected DRepository(Enum enumType) : this(enumType.ToString())
        {
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <param name="threadCache"></param>
        /// <returns></returns>
        protected IDbConnection GetConnection(Enum connectionName, bool threadCache = true)
        {
            return _factory.Connection(connectionName, threadCache);
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <param name="threadCache"></param>
        /// <returns></returns>
        protected IDbConnection GetConnection(string connectionName = null, bool threadCache = true)
        {
            var name = string.IsNullOrWhiteSpace(connectionName) ? _defaultConnectionName : connectionName;
            return _factory.Connection(name, threadCache);
        }

        /// <summary> 执行数据库事务 </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="connectionName"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected TResult Transaction<TResult>(Func<IDbConnection, IDbTransaction, TResult> action, string connectionName = null,
            IsolationLevel? level = null)
        {
            var name = string.IsNullOrWhiteSpace(connectionName) ? _defaultConnectionName : connectionName;
            var conn = GetConnection(name, false);
            using (conn)
            {
                conn.Open();
                var transaction = level.HasValue ? conn.BeginTransaction(level.Value) : conn.BeginTransaction();
                using (transaction)
                {
                    try
                    {
                        var result = action.Invoke(conn, transaction);
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary> 执行数据库事务 </summary>
        /// <param name="action"></param>
        /// <param name="connectionName"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected DResult Transaction(Action<IDbConnection, IDbTransaction> action, string connectionName = null,
            IsolationLevel? level = null)
        {
            var result = Transaction((conn, trans) =>
            {
                action.Invoke(conn, trans);
                return DResult.Success;
            }, connectionName, level);
            return result ?? DResult.Error("事务执行失败");
        }

        /// <summary> 更新数量 </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <param name="count"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        protected int Increment<T>(string column, object key, string keyColumn = "id",
            int count = 1, IDbConnection conn = null, IDbTransaction trans = null)
        {
            return (conn ?? Connection).Increment<T>(column, key, keyColumn, count, trans);
        }
    }
}
