using Acb.Core.Domain;
using Acb.Core.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Acb.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbConnectionProvider ConnectionProvider { private get; set; }
        private readonly string _configName;
        private readonly string _connectionString;
        private readonly string _providerName;


        private readonly ILogger _logger;

        public UnitOfWork(string configName = null)
        {
            _configName = configName;
            _logger = LogManager.Logger<UnitOfWork>();
        }

        public UnitOfWork(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
        }

        private IDbConnection _connection;
        /// <summary> 当前数据库连接 </summary>
        public IDbConnection Conntection
        {
            get
            {
                if (_connection != null)
                    return _connection;
                _logger.Debug($"UnitOfWork Create Connection [{GetType().Name}]");
                if (!string.IsNullOrWhiteSpace(_connectionString))
                    return _connection = ConnectionProvider.Connection(_connectionString, _providerName);
                return _connection = ConnectionProvider.Connection(_configName);
            }
        }

        private IDbTransaction _transaction;

        /// <summary> 当前事务 </summary>
        public IDbTransaction Transaction => _transaction;

        public bool IsTransaction => _transaction != null;

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
            _logger.Debug("UnitOfWork Create Transaction");
            var wasCloesd = Conntection.State == ConnectionState.Closed;
            if (wasCloesd)
                Conntection.Open();
            _transaction = level.HasValue ? Conntection.BeginTransaction(level.Value) : Conntection.BeginTransaction();
            try
            {
                var result = func.Invoke();
                var task = result as Task;
                task?.GetAwaiter().GetResult();
                _transaction.Commit();
                _logger.Debug("UnitOfWork Transaction Commit");
                return result;
            }
            catch
            {
                _transaction.Rollback();
                _logger.Warn("UnitOfWork Transaction Rollback");
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
                _logger.Debug("UnitOfWork Transaction Dispose");
                if (wasCloesd)
                    Conntection.Close();
            }
        }

        public void Dispose()
        {
            if (_connection == null) return;
            _logger.Debug("UnitOfWork Dispose");
            _connection?.Dispose();
            _transaction?.Dispose();
        }
    }
}
