using Acb.Core;
using Acb.Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class DapperTest : DTest
    {
        [TestMethod]
        public void PagedTest()
        {
            //SQL insert = "insert into [mscreen_user] () values (@id,@name)";
            SQL sql = "select * from [mscreen_user] where [Name]=@name";
            using (var conn = ConnectionFactory.Instance.Connection("default", false))
            {
                var list = sql.PagedList<dynamic>(conn, 1, 10, new { name = "shay" });
                Print(DResult.Succ(list, list.Total));
            }
        }


        [TestMethod]
        public void UpdateTest()
        {
            using (var conn = ConnectionFactory.Instance.Connection("default", false))
            {
            }
        }
    }
}
