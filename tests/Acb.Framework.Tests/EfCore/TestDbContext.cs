using Acb.EntityFramework;
using Acb.Framework.Tests.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Acb.Framework.Tests.EfCore
{
    internal class TestDbContext : AcbDbContext
    {

        public DbSet<TAreas> TAreas { get; set; }

        public TestDbContext(DbContextOptions options)
        {
        }

        
    }
}
