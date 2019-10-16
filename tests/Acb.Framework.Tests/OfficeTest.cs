using Acb.Core.Dependency;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Dapper;
using Acb.Demo.Business.Domain.Entities;
using Acb.Office;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acb.Core.Data;
using MongoDB.Bson;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class OfficeTest : DTest
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public OfficeTest()
        {
            _connectionProvider = CurrentIocManager.Resolve<IDbConnectionProvider>();
        }
        [TestMethod]
        public void CreateTest()
        {
            //var dt = new DataTable("test");
            //dt.Columns.AddRange(new[] { new DataColumn("姓名"), new DataColumn("手机号", typeof(int)) });
            //for (var i = 0; i < 5; i++)
            //{
            //    dt.Rows.Add($"姓名{i}", RandomHelper.Random().Next(10000, 30000));
            //}
            using (var conn = _connectionProvider.Connection())
            {
                var excepts = new[] { nameof(TAreas.Deep) };
                var columns = typeof(TAreas).Columns(excepts);
                //columns = conn.FormatSql(columns);
                //var dt = conn.Query<DataTable>($"select {columns} from `t_areas` where `parent_code`=@code",
                //    new { code = "510700" });
                //Print(dt);
                var list = conn.Query<TAreas>($"select `city_code` as `Id` from `t_areas` where `parent_code`=@code",
                    new { code = "510700" });
                var names = new Dictionary<string, string>
                {
                    {nameof(TAreas.Id), "城市编码"},
                    {nameof(TAreas.CityName), "城市名称"},
                    {nameof(TAreas.Deep), "深度"},
                    {nameof(TAreas.ParentCode), "父级编码"}
                };
                var dt = list.ToDataTable(name => names.ContainsKey(name) ? names[name] : name, excepts: excepts);
                ExcelHelper.CreateFile(new DataSet { Tables = { dt } },
                    Path.Combine(Directory.GetCurrentDirectory(), "test.xlsx"));
            }
        }

        [TestMethod]
        public async Task ExportTest()
        {
            const string gateway = "http://open.i-cbao.com/jk/many-vehicle-mileage";
            const string xls = "d://导出4S店车辆里程数据.xls";
            const int size = 10;
            var dt = ExcelHelper.ReadFirst(xls);
            var rows = dt.Rows.Count;
            Print(rows);
            LogManager.SetLogLevel(LogLevel.Off);
            var page = (int)Math.Ceiling(rows / (float)size);
            for (var i = 0; i < page; i++)
            {
                var list = new Dictionary<string, DataRow>();
                for (var j = 0; j < size; j++)
                {
                    var index = i * size + j;
                    if (index > rows - 1)
                        break;
                    var row = dt.Rows[index];
                    list.Add(row[0].ToString(), row);
                }

                var resp = await HttpHelper.Instance.PostAsync(gateway, new
                {
                    ArrVehicleId = list.Keys.ToList()
                });
                var html = await resp.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<dynamic>(html);
                foreach (var item in results.data)
                {
                    var key = (string)item.vehicleId;
                    if (list.ContainsKey(key))
                        list[key][7] = item.mileage;
                }
            }
            ExcelHelper.CreateFile(new DataSet { Tables = { dt } }, "d://data.xls");
        }

        [TestMethod]
        public void ReadTest()
        {
            const string path = "E:\\test.xls";
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var dt = ExcelHelper.ReadFirst(stream);
                foreach (DataRow row in dt.Rows)
                {
                    Print(row.ItemArray);
                }
            }
        }
    }
}
