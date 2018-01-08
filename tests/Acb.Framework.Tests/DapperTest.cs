using Acb.Core;
using Acb.Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class DapperTest : DTest
    {
        public DapperTest() : base(Assembly.GetExecutingAssembly())
        {
        }

        [TestMethod]
        public void PagedTest()
        {
            //SQL insert = "insert into [mscreen_user] () values (@id,@name)";
            SQL sql = "select * from [mscreen_user] where [Name]=@name";
            using (var conn = ConnectionFactory.Instance.Connection("default", false))
            {
                var list = sql.PagedList<dynamic>(conn, 1, 10, new { name = "shay" });
                Print(DResult.Succ(list, list.TotalCount));
            }
        }
    }
}
