using Acb.Core.Data;

namespace Acb.WebApi.Test.Connections
{
    public class MainDbContext : UnitOfWork
    {
        public MainDbContext() : base("icb_main")
        {
        }
    }
}
