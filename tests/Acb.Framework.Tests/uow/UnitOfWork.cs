using System;
using System.Data;

namespace Acb.Framework.Tests.uow
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly Guid _id;

        internal UnitOfWork(IDbConnection connection)
        {
            _id = Guid.NewGuid();
            _connection = connection;
        }

        IDbConnection IUnitOfWork.Connection => _connection;

        IDbTransaction IUnitOfWork.Transaction => _transaction;

        Guid IUnitOfWork.Id => _id;

        public void Begin(IsolationLevel? level = null)
        {
            _transaction = level.HasValue ? _connection.BeginTransaction(level.Value) : _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            Dispose();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
