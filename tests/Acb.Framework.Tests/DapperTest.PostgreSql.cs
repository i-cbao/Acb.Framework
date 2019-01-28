using Acb.Core.Data;
using Acb.Core.Domain;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    public partial class DapperTest
    {
        [TestMethod]
        public async Task AsyncTest()
        {
            var uw = Resolve<IUnitOfWork>();

            const string sql = "select * from [t_account] where [Account]=@account";
            //await uw.Trans(async () =>
            //{


            using (var conn = uw.CreateConnection())
            {
                var fsql = conn.FormatSql(sql);
                //var factory = Resolve<IDbConnectionProvider>();
                //var a1 = factory.Connection(conn.ConnectionString, "postgresql")
                //    .QueryFirstOrDefaultAsync(fsql, new { account = "ichebao" });
                //var a2 = factory.Connection(conn.ConnectionString, "postgresql")
                //    .QueryFirstOrDefaultAsync(fsql, new { account = "icbhs" });
                var a1 = await conn.QueryFirstOrDefaultAsync(fsql, new { account = "ichebao" });
                var a2 = await conn.QueryFirstOrDefaultAsync(fsql, new { account = "icbhs" });
                Print(a1);
                Print(a2);
            }

            //});
            //var areas = _demoService.Areas("110000");
            //var result = _demoService.Update();
            //Print(await areas);
            //Print(await result);
        }
    }
}
