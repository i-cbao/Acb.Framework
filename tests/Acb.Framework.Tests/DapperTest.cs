using Acb.Core;
using Acb.Dapper;
using NUnit.Framework;
using System.Reflection;

namespace Acb.Framework.Tests
{
    public class DapperTest : DTest
    {
        public DapperTest() : base(Assembly.GetExecutingAssembly())
        {
        }

        [Test]
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


        [Test]
        public void UpdateTest()
        {
            using (var conn = ConnectionFactory.Instance.Connection("default", false))
            {
            }
        }
    }
}
