using Acb.Core.Data;
using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Acb.Dapper.Domain
{
    public abstract class DRepository : IDependency
    {
        /// <summary> 数据库连接提供者 </summary>
        public IUnitOfWork UnitOfWork { get; }

        /// <summary> 获取默认连接 </summary>
        protected IDbConnection Connection => UnitOfWork.Connection;
        /// <summary> 当前事务 </summary>
        protected IDbTransaction Trans => UnitOfWork.Transaction;

        public IDbConnectionProvider ConnectionProvider { protected get; set; }

        protected DRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            LogManager.Logger<DRepository>().Info("DRepository Create");
        }

        protected DRepository() : this(CurrentIocManager.Resolve<UnitOfWork>())
        {
        }

        /// <summary> 建议使用Ioc注入的方式 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("建议使用Ioc注入的方式")]
        public static T Instance<T>() where T : DRepository
        {
            return CurrentIocManager.Resolve<T>();
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        protected IDbConnection GetConnection(Enum connectionName)
        {
            return ConnectionProvider.Connection(connectionName);
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        protected IDbConnection GetConnection(string connectionName = null)
        {
            return ConnectionProvider.Connection(connectionName);
        }

        /// <summary> 执行事务(当前连接) </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected TResult Transaction<TResult>(Func<TResult> action, IsolationLevel? level = null)
        {
            return UnitOfWork.BeginTransaction(action, level);
        }

        /// <summary> 执行事务(当前连接) </summary>
        /// <param name="action"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected void Transaction(Action action, IsolationLevel? level = null)
        {
            UnitOfWork.BeginTransaction(action, level);
        }

        /// <summary> 执行事务(新开连接) </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="level"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        protected TResult Transaction<TResult>(Func<IDbConnection, IDbTransaction, TResult> action,
             string connectionName = null, IsolationLevel? level = null)
        {
            using (var conn = GetConnection(connectionName))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (var trans = level.HasValue ? conn.BeginTransaction(level.Value) : conn.BeginTransaction())
                {
                    try
                    {
                        var result = action.Invoke(conn, trans);
                        var task = result as Task;
                        task?.GetAwaiter().GetResult();
                        trans.Commit();
                        return result;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary> 执行事务(新开连接) </summary>
        /// <param name="action"></param>
        /// <param name="level"></param>
        /// <param name="connectionName"></param>
        protected void Transaction(Action<IDbConnection, IDbTransaction> action, string connectionName = null,
            IsolationLevel? level = null)
        {
            Transaction((conn, trans) =>
            {
                action.Invoke(conn, trans);
                return true;
            }, connectionName, level);
        }

        /// <summary> 更新数量 </summary>
        /// <param name="column"></param>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected int Increment<T>(string column, object key, string keyColumn = "id",
            int count = 1)
        {
            return Connection.Increment<T>(column, key, keyColumn, count, Trans);
        }
    }
}
