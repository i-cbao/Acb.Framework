using Acb.Core.Domain;
using Acb.Core.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Acb.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary> 连接提供者 </summary>
        //public IDbConnectionProvider ConnectionProvider { private get; set; }

        public ConnectionFactory ConnectionFactory { private get; set; }

        private readonly string _configName;
        private readonly string _connectionString;
        private readonly string _providerName;

        private readonly ILogger _logger;

        public UnitOfWork(string configName = null)
        {
            _configName = configName;
            _logger = LogManager.Logger<UnitOfWork>();
            _logger.Info("Create UnitOfWork");
        }

        public UnitOfWork(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
            _logger.Info("Create UnitOfWork");
        }

        private IDbConnection _connection;

        /// <inheritdoc />
        /// <summary> 当前数据库连接 </summary>
        public IDbConnection Connection
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_connectionString))
                    return _connection = ConnectionFactory.Connection(_connectionString, _providerName);
                return _connection = ConnectionFactory.Connection(_configName);
                //if (_connection == null)
                //{
                //    lock (LockObj)
                //    {
                //        if (_connection == null)
                //        {
                //            //var factory = CurrentIocManager.Resolve<ConnectionFactory>();
                //            //if (!string.IsNullOrWhiteSpace(_connectionString))
                //            //    _connection = factory.Connection(_connectionString, _providerName);
                //            //_connection = factory.Connection(_configName);
                //            if (!string.IsNullOrWhiteSpace(_connectionString))
                //                return _connection = ConnectionProvider.Connection(_connectionString, _providerName);
                //            return _connection = ConnectionProvider.Connection(_configName);
                //        }
                //    }
                //}

                //return _connection;
            }
        }

        /// <summary> 当前事务 </summary>
        public IDbTransaction Transaction { get; private set; }

        public bool IsTransaction => Transaction != null;

        /// <summary> 执行事务 </summary>
        /// <param name="action"></param>
        /// <param name="level"></param>
        public void BeginTransaction(Action action, IsolationLevel? level = null)
        {
            BeginTransaction(() =>
            {
                action();
                return true;
            }, level);
        }

        /// <summary> 执行事务 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public T BeginTransaction<T>(Func<T> func, IsolationLevel? level = null)
        {
            var wasCloesd = Connection.State == ConnectionState.Closed;
            if (wasCloesd)
                Connection.Open();
            var disposed = false;
            if (Transaction == null)
            {
                disposed = true;
                Transaction = level.HasValue
                    ? Connection.BeginTransaction(level.Value)
                    : Connection.BeginTransaction();
                _logger.Debug("UnitOfWork Create Transaction");
            }

            try
            {
                var result = func.Invoke();
                var task = result as Task;
                task?.GetAwaiter().GetResult();
                if (disposed)
                {
                    Transaction.Commit();
                    _logger.Debug("UnitOfWork Transaction Commit");
                }

                return result;
            }
            catch
            {
                if (disposed)
                {
                    Transaction.Rollback();
                    _logger.Warn("UnitOfWork Transaction Rollback");
                }

                throw;
            }
            finally
            {
                if (disposed)
                {
                    Transaction.Dispose();
                    Transaction = null;
                    _logger.Debug("UnitOfWork Transaction Dispose");
                }

                if (wasCloesd)
                    Connection.Close();
            }
        }

        public void Dispose()
        {
            if (_connection == null) return;
            _logger.Debug("UnitOfWork Dispose");
            Transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
