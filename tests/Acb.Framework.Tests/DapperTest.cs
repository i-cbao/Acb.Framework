using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Tests;
using Acb.Dapper;
using Acb.Demo.Business.Domain;
using Acb.Demo.Business.Domain.Entities;
using Acb.Demo.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MongoDB.Bson;

namespace Acb.Framework.Tests
{
    [TestClass]
    public partial class DapperTest : DTest
    {
        private readonly AreaRepository _areaRepository;
        private readonly IDbConnectionProvider _factory;
        private readonly IDemoService _demoService;

        public DapperTest()
        {
            //_areaRepository = Resolve<AreaRepository>();
            //_factory = Resolve<IDbConnectionProvider>();
            //_demoService = Resolve<IDemoService>();
        }

        protected override void MapServices(IServiceCollection services)
        {
            services.AddDapper(config =>
            {
                config.ConnectionString =
                    "server=192.168.0.250;user=root;database=icbv2db;port=3306;password=icb@888;Pooling=true;SslMode=none;Charset=utf8;";
                config.ProviderName = "mysql";
                //config.ConnectionString =
                //    "Host=192.168.0.251;Port=15432;Username=postgres;Password=;Database=db_spear;Pooling=true;Minimum Pool Size=10;Maximum Pool Size=300;";
                //config.ProviderName = "postgresql";
            });
        }

        [TestMethod]
        public async Task PagedTest()
        {

            var columns = typeof(TAreas).Columns();
            var sql = $"select {columns} from [t_areas] where [parent_code]=@code";
            var conn = _factory.Connection("default");
            using (conn)
            {
                //var set = conn.QueryDataSet(conn.FormatSql(sql), new { code = "510100" });
                //Print(set);
                var list = conn.PagedListAsync<TAreas>(sql, 2, 6, new { code = "510100" }).Result;
                //var mapper = _config.CreateMapper();
                //var dtos = Mapper.Map<PagedList<TAreas>>(list);
                //Print(dtos);
                Print(DResult.Succ(list));
            }

            var t = await conn.QueryByIdAsync<TAreas>("510000");
            Print(t);
        }

        [TestMethod]
        public async Task QueryTest()
        {
            var r = await CodeTimer.Time("thread test", 5, async () =>
             {
                 var model = await _areaRepository.Get("110108");
                 Print($"Thread Id:{Thread.CurrentThread.ManagedThreadId}");
                 Print(model);
             });
            Print(r.ToString());
            var t = await _areaRepository.Get("110108");
            Print(t);
            Print(CurrentIocManager.Resolve<ConnectionFactory>().ToString());
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var demo = Resolve<IDemoService>();
            var result = await demo.Update();
            var list = await demo.Areas("510100");
            Print(result);
            Print(list);
            //var code = await CodeTimer.Time("dapper", 10, async () =>
            //{
            //    var demo = Resolve<IDemoService>();
            //    var result = await demo.Update();
            //    var list = await demo.Areas("510100");
            //}, 2);
            ////Print(list);
            //Print(code.ToString());
            //using (var conn = _factory.Connection())
            //{
            //    var result = conn.Update(new TAreas
            //    {
            //        Id = "110000",
            //        CityName = "北京市",
            //        ParentCode = "0",
            //        Deep = 1
            //    });
            //    Print(result);
            //}
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

        [TestMethod]
        public async Task ConnectionTest()
        {
            using (var scope = new LifetimeScopeManager())
            {
                //var uw = scope.Resolve<IUnitOfWork>();
                var result = await CodeTimer.Time("test", 10, async () =>
                {
                    var resp = scope.Resolve<AreaRepository>();
                    var list = await resp.QueryAreaAsync("510000");
                    using (var conn = resp.UnitOfWork.Connection)
                    {
                        var t = await conn.QueryByIdAsync<TAreas>("510000");
                        //Print(t);
                    }

                    //Print(await list);
                }, 20);
                Print(result.ToString());
            }
        }

        [TestMethod]
        public void SqlBuilderTest()
        {
            Print(0x10000 | 0x80);
            const string countSql = "SELECT COUNT(1) FROM [t_areas] /**where**/";
            const string selectSql = "SELECT * FROM [t_areas] /**where**/ /**orderby**/";
            var builder = new SqlBuilder();
            //builder.Select("[parent_code]=@parentCode", new { parentCode = "510000" });
            builder.OrWhere("[city_code]=@code", new { code = "510000" });
            builder.OrWhere("[parent_code]=@parentCode", new { parentCode = "510000" });
            //builder.Select("[city_code]=@code", new { code = "510000" });
            builder.OrderBy("[name] DESC");

            var c = builder.AddTemplate(countSql);
            var sel = builder.AddTemplate(selectSql);
            Print(c.RawSql);
            Print(c.Parameters);
            Print(sel.RawSql);
            Print(sel.Parameters);
        }

        [TestMethod]
        public void SqlTest()
        {
            var sql = typeof(TAreas).Select(" [parent_code]=@code", " [city_code]",
                includes: new[] { nameof(TAreas.CityName), nameof(TAreas.Id) });
            using (var conn = Resolve<IUnitOfWork>().CreateConnection())
            {
                var fsql = conn.FormatSql(sql);
                var t = conn.Query<TAreas>(fsql, new { code = "510000" });
                Print(t);
            }
        }
    }
}
