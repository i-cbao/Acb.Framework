using Acb.Dapper;

namespace Acb.WebApi.Test.Connections
{
    public class MainDbContext : UnitOfWork
    {
        public MainDbContext() : base("icb_main")
        {
        }
    }
}
