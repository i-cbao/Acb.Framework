using Acb.Core.Data;
using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Logging;
using System;
using System.Data;

namespace Acb.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionProvider _factory;
        private readonly ILogger _logger;

        private bool _closeabel;

        public Guid Id { get; }
        private readonly string _configName;

        private readonly string _connectionString;
        private readonly string _providerName;

        private static readonly object SyncObj = new object();

        private UnitOfWork()
        {
            Id = Guid.NewGuid();
            _factory = CurrentIocManager.Resolve<IDbConnectionProvider>();
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

        private IDbConnection _connection;
        /// <inheritdoc />
        /// <summary> 当前数据库连接 </summary>
        public IDbConnection Connection
        {
            get
            {
                lock (SyncObj)
                {
                    if (_connection == null)
                    {
                        return _connection = CreateConnection();
                    }
                    return _connection;
                }
            }
        }

        /// <inheritdoc />
        /// <summary> 当前事务 </summary>
        public IDbTransaction Transaction { get; private set; }

        public bool IsTransaction => Transaction != null;

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

        public IDbConnection CreateConnection()
        {
            return string.IsNullOrWhiteSpace(_connectionString)
                ? _factory.Connection(_configName)
                : _factory.Connection(_connectionString, _providerName);
        }

        public void Commit()
        {
            Transaction?.Commit();
            _logger.Debug($"{GetType().Name}[{Id}] Commit Transaction");
            Dispose();
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            _logger.Debug($"{GetType().Name}[{Id}] Rollback Transaction");

            Dispose();
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                _logger.Debug($"{GetType().Name}[{Id}] Dispose Transaction");
                Transaction.Dispose();
                Transaction = null;
            }
            if (_closeabel)
                Connection.Close();
        }
    }
}
