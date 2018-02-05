using Acb.Core;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Framework.Tests.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class DapperTest : DTest
    {
        private readonly AreaRepository _areaRepository;

        public DapperTest()
        {
            _areaRepository = DRepository.Instance<AreaRepository>();
        }

        [TestMethod]
        public void PagedTest()
        {
            const string sql = "select * from [t_areas] where [parent_code]=@code";
            using (var conn = ConnectionFactory.Instance.Connection("default", false))
            {
                var list = conn.PagedList<dynamic>(sql, 2, 6, new { code = "510100" });
                Print(DResult.Succ(list, list.Total));
            }
        }

        [TestMethod]
        public async Task QueryTest()
        {
            //var sql = string.Empty;
            //var result = CodeTimer.Time("sql test", 20000, () =>
            //{
            //    sql = typeof(TAreas).InsertSql(new[] { nameof(TAreas.CityCode) });
            //}, 10);
            //Print(result.ToString());
            //Print(sql);
            var model = await _areaRepository.Get("110108");
            Print(model);
        }

        [TestMethod]
        public void UpdateTest()
        {
            using (var conn = ConnectionFactory.Instance.Connection(threadCache: false))
            {
                var result = conn.Update(new TAreas
                {
                    CityCode = "",
                    CityName = ""
                }, new[] { nameof(TAreas.CityName) });
            }
        }
    }
}
