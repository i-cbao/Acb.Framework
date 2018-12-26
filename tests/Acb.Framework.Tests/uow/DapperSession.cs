using Acb.Core.Data;
using Acb.Core.Dependency;
using System;
using System.Data;

namespace Acb.Framework.Tests.uow
{
    public sealed class DapperSession : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IDbConnectionProvider _factory;

        private DapperSession()
        {
            _factory = CurrentIocManager.Resolve<IDbConnectionProvider>();
        }

        public DapperSession(string connectionString, string provider) : this()
        {
            _connection = _factory.Connection(connectionString, provider);
            _connection.Open();
            UnitOfWork = new UnitOfWork(_connection);
        }

        public DapperSession(string configName) : this()
        {
            _connection = _factory.Connection(configName);
            _connection.Open();
            UnitOfWork = new UnitOfWork(_connection);
        }

        public DapperSession(Enum configName) : this()
        {
            _connection = _factory.Connection(configName);
            _connection.Open();
            UnitOfWork = new UnitOfWork(_connection);
        }

        public IUnitOfWork UnitOfWork { get; }

        public void Dispose()
        {
            UnitOfWork.Dispose();
            _connection.Dispose();
        }
    }
}
