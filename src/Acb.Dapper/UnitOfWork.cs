using Acb.Core.Data;
using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;

namespace Acb.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionProvider _factory;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<int, Lazy<IDbConnection>> _connections;

        private bool _closeabel;

        /// <inheritdoc />
        public Guid Id { get; }
        private readonly string _configName;

        private readonly string _connectionString;
        private readonly string _providerName;

        //private static readonly object SyncObj = new object();

        private UnitOfWork()
        {
            Id = Guid.NewGuid();
            _factory = CurrentIocManager.Resolve<IDbConnectionProvider>();
            _connections = new ConcurrentDictionary<int, Lazy<IDbConnection>>();
            _logger = LogManager.Logger(GetType());
            _logger.Debug($"{GetType().Name}:{Id} Create");
        }

        public UnitOfWork(string configName = null) : this()
        {
            _configName = configName;
        }

        public UnitOfWork(string connectionString, string providerName) : this()
        {
            _connectionString = connectionString;
            _providerName = providerName;
        }

        /// <inheritdoc />
        /// <summary> 当前数据库连接 </summary>
        public IDbConnection Connection
        {
            get
            {
                //return CreateConnection();
                var key = Thread.CurrentThread.ManagedThreadId;
                _logger.Debug($"{GetType().Name}[{Id}],Current Thread:{key},{_connections.Count}");
                var lazy = _connections.GetOrAdd(key, k => new Lazy<IDbConnection>(CreateConnection));
                return lazy.Value;
            }
        }

        /// <inheritdoc />
        /// <summary> 当前事务 </summary>
        public IDbTransaction Transaction { get; private set; }

        /// <summary> 是否开启事务 </summary>
        public bool IsTransaction => Transaction != null;

        /// <summary> 开始事务 </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Begin(IsolationLevel? level = null)
        {
            if (IsTransaction)
                return false;
            var conn = Connection;
            _closeabel = conn.State == ConnectionState.Closed;
            if (_closeabel)
                conn.Open();
            _logger.Debug($"{GetType().Name}[{Id}] Begin Transaction");
            Transaction = level.HasValue
                ? conn.BeginTransaction(level.Value)
                : conn.BeginTransaction();
            return true;
        }

        /// <summary> 创建连接 </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            return string.IsNullOrWhiteSpace(_connectionString)
                ? _factory.Connection(_configName)
                : _factory.Connection(_connectionString, _providerName);
        }

        /// <summary> 提交事务 </summary>
        public void Commit()
        {
            Transaction?.Commit();
            _logger.Debug($"{GetType().Name}[{Id}] Commit Transaction");
            Dispose();
        }

        /// <summary> 回滚事务 </summary>
        public void Rollback()
        {
            Transaction?.Rollback();
            _logger.Debug($"{GetType().Name}[{Id}] Rollback Transaction");

            Dispose();
        }

        /// <summary> 资源释放 </summary>
        public void Dispose()
        {
            _logger.Debug($"{GetType().Name}[{Id}] Dispose UnitOfWork");
            if (Transaction != null)
            {
                _logger.Debug($"{GetType().Name}[{Id}] Dispose Transaction");
                Transaction.Dispose();
                Transaction.Connection.Close();
                Transaction = null;
            }

            if (_connections.Count > 0)
            {
                foreach (var conn in _connections.Values)
                {
                    conn.Value.Close();
                }
            }

            _connections.Clear();
            if (_closeabel)
                Connection.Close();
        }
    }
}
