using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Dependency;
using Acb.Core.Tests;
using Acb.Dapper;
using Acb.Demo.Business.Domain;
using Acb.Demo.Business.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class DapperTest : DTest
    {
        private readonly AreaRepository _areaRepository;
        private readonly IDbConnectionProvider _factory;

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
            using (var conn = _factory.Connection("default"))
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
            var r = CodeTimer.Time("thread test", 5, async () =>
             {
                 var model = await _areaRepository.Get("110108");
                 Print($"Thread Id:{Thread.CurrentThread.ManagedThreadId}");
                 Print(model);
             });
            Print(r.ToString());
            var t = await _areaRepository.Get("110108");
            Print(t);
            Print(CurrentIocManager.Resolve<IDbConnectionProvider>().ToString());
        }

        [TestMethod]
        public void UpdateTest()
        {
            using (var conn = _factory.Connection())
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
            using (var conn = _factory.Connection())
            {
                sql.Paged(1, 15, conn);
                var t = sql.ToString();
                Print(t);
            }

        }
    }
}
