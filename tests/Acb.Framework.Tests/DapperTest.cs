using Acb.Core;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Framework.Tests.Repositories;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class DapperTest : DTest
    {
        private readonly AreaRepository _areaRepository;

        public class AreaDto
        {
            public string city_code { get; set; }
            public string city_name { get; set; }
            public string parent_code { get; set; }
            public int deep { get; set; }
        }

        public DapperTest()
        {
            _areaRepository = DRepository.Instance<AreaRepository>();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
            });
        }

        [TestMethod]
        public void PagedTest()
        {
            var columns = typeof(TAreas).Columns();
            var sql = $"select {columns} from [t_areas] where [parent_code]=@code";
            using (var conn = ConnectionFactory.Instance.Connection("default", false))
            {
                var list = conn.PagedList<TAreas>(sql, 2, 6, new { code = "510100" });
                //var mapper = _config.CreateMapper();
                //var dtos = Mapper.Map<PagedList<TAreas>>(list);
                //Print(dtos);
                Print(DResult.Succ(list.List, list.Total));
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
