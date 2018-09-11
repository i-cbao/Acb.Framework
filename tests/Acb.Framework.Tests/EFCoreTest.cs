using Acb.Core.Data.Config;
using Acb.Core.Extensions;
using Acb.Framework.Tests.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class EfCoreTest : DTest
    {
        private readonly IServiceProvider _serviceProvider;
        public EfCoreTest()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            services.AddDbContext<TestDbContext>(optons =>
            {
                var connect = "dapper:default".Config<ConnectionConfig>();
                optons.UseMySQL(connect.ConnectionString);
            });
            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task AreaTest()
        {
            using (var db = _serviceProvider.GetService<TestDbContext>())
            {
                var areas = await db.TAreas.Where(t => t.ParentCode == "510000").ToListAsync();
                Print(areas);
            }
        }
    }
}
