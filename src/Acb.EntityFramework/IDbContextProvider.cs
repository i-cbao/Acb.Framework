using Acb.Core.Dependency;
using Acb.Core.Domain;

namespace Acb.EntityFramework
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : IUnitOfWork
    {
        TDbContext DbContext { get; }
    }
}
