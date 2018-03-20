using Acb.Core.Dependency;
using Acb.Core.Domain;

namespace Acb.EntityFramework
{
    public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : IUnitOfWork
    {
        //private static readonly ConcurrentDictionary<string, IUnitOfWork> UnitOfWorkDictionary;
        //private static readonly object LockObj = new object();

        //static UnitOfWorkDbContextProvider()
        //{
        //    UnitOfWorkDictionary = new ConcurrentDictionary<string, IUnitOfWork>();
        //}

        public TDbContext DbContext => CurrentIocManager.Resolve<TDbContext>();
    }
}
