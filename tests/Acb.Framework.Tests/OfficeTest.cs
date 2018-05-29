using Acb.Dapper;
using Acb.Dapper.Adapters;
using Acb.Framework.Tests.Repositories;
using Acb.Office;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class OfficeTest : DTest
    {
        [TestMethod]
        public void CreateTest()
        {
            //var dt = new DataTable("test");
            //dt.Columns.AddRange(new[] { new DataColumn("姓名"), new DataColumn("手机号", typeof(int)) });
            //for (var i = 0; i < 5; i++)
            //{
            //    dt.Rows.Add($"姓名{i}", RandomHelper.Random().Next(10000, 30000));
            //}
            using (var conn = ConnectionFactory.Instance.Connection(threadCache: false))
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
    }
}
