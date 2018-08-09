using Acb.Core;
using Acb.Core.Dependency;
using Acb.Dapper;
using Acb.Framework.Tests.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class DapperTest : DTest
    {
        private readonly AreaRepository _areaRepository;
        private readonly IDbConnectionProvider _factory;

        public class AreaDto
        {
            public string city_code { get; set; }
            public string city_name { get; set; }
            public string parent_code { get; set; }
            public int deep { get; set; }
        }

        public DapperTest()
        {
            _areaRepository = CurrentIocManager.Resolve<AreaRepository>();
            _factory = CurrentIocManager.Resolve<IDbConnectionProvider>();
        }

        [TestMethod]
        public void PagedTest()
        {

            var columns = typeof(TAreas).Columns();
            var sql = $"select {columns} from [t_areas] where [parent_code]=@code";
            using (var conn = _factory.Connection("default", false))
            {
                //var set = conn.QueryDataSet(conn.FormatSql(sql), new { code = "510100" });
                //Print(set);
                var list = conn.PagedListAsync<TAreas>(sql, 2, 6, new { code = "510100" }).Result;
                //var mapper = _config.CreateMapper();
                //var dtos = Mapper.Map<PagedList<TAreas>>(list);
                //Print(dtos);
                Print(DResult.Succ(list));
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
            using (var conn = _factory.Connection(threadCache: false))
            {
                var result = conn.Update(new TAreas
                {
                    Id = "110000",
                    CityName = "北京市",
                    ParentCode = "0",
                    Deep = 1
                });
                Print(result);
            }
        }

        [TestMethod]
        public void SqlPagedTest()
        {
            const string str = @"SELECT
  s.`id` AS ShopId,
  s.`name` AS ShopName,
  Count(u.id) AS Total,
  0 AS DownLoadedUser,
  0 AS UnDownLoadUser
FROM
  `t_shop` AS s
LEFT JOIN `t_user` AS u ON s.`id` = u.`shop_id`
WHERE
  u.state !=- 400
AND u.create_time >=""2018 / 05 / 10""
            AND u.create_time < ""2018/05/13""
            GROUP BY
            s.`id`
            ORDER BY
            Count(u.`id`) DESC";
            SQL sql = str;
            using (var conn = _factory.Connection(threadCache: false))
            {
                sql.Paged(1, 15, conn);
                var t = sql.ToString();
                Print(t);
            }

        }
    }
}
