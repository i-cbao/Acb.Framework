using System;
using System.Data;

namespace Acb.Framework.Tests.uow
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Begin(IsolationLevel? level = null);
        void Commit();
        void Rollback();
    }
}
