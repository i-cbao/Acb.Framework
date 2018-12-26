using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Logging;
using System;
using System.Data;

namespace Acb.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ConnectionFactory _factory;
        private readonly ILogger _logger;

        private bool _closeabel;

        public Guid Id { get; }
        public string ConfigName { get; }

        private UnitOfWork()
        {
            Id = Guid.NewGuid();
            _factory = CurrentIocManager.Resolve<ConnectionFactory>();
            _logger = LogManager.Logger(GetType());
            _logger.Debug($"{GetType().Name}:{Id} Create");
        }

        public UnitOfWork(string configName = null) : this()
        {
            ConfigName = configName;
            Connection = _factory.Connection(configName, false);
        }

        public UnitOfWork(string connectionString, string providerName) : this()
        {
            Connection = _factory.Connection(connectionString, providerName, false);
        }

        /// <inheritdoc />
        /// <summary> 当前数据库连接 </summary>
        public IDbConnection Connection { get; }

        /// <inheritdoc />
        /// <summary> 当前事务 </summary>
        public IDbTransaction Transaction { get; private set; }

        public bool IsTransaction => Transaction != null;

        public bool Begin(IsolationLevel? level = null)
        {
            if (IsTransaction)
                return false;
            _closeabel = Connection.State == ConnectionState.Closed;
            if (_closeabel)
                Connection.Open();
            _logger.Debug($"{GetType().Name}[{Id}] Begin");
            Transaction = level.HasValue
                ? Connection.BeginTransaction(level.Value)
                : Connection.BeginTransaction();
            return true;
        }

        public void Commit()
        {
            Transaction?.Commit();
            _logger.Debug($"{GetType().Name}[{Id}] Commit");
            Dispose();
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            _logger.Debug($"{GetType().Name}[{Id}] Rollback");

            Dispose();
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                _logger.Debug($"{GetType().Name}[{Id}] Dispose");
                Transaction.Dispose();
                Transaction = null;
            }
            if (_closeabel)
                Connection.Close();
        }
    }
}
