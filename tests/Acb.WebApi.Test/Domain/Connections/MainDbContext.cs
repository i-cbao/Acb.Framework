using Acb.Dapper;

namespace Acb.WebApi.Test.Domain.Connections
{
    public class MainDbContext : UnitOfWork
    {
        public MainDbContext() : base("icb_main")
        {
        }
    }
}
