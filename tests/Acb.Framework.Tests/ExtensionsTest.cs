using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Tests;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Demo.Business.Domain.Entities;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Acb.Framework.Tests
{
    public enum EnumType
    {
        [System.ComponentModel.Description("AA")]
        A = 1,
        [System.ComponentModel.Description("BB")]
        B = 3
    }

    [TestClass]
    public class ExtensionsTest : DTest
    {
        [TestMethod]
        public void EnumTest()
        {
            var text = (EnumType.A | EnumType.B).GetText(true);
            Print(text);
        }

        [TestMethod]
        public void CheckPropsTest()
        {
            var source = new
            {
                Id = "1234",
                CityName = "北京",
                //ParentCode = "10010",
                Deep = 0
            };
            var target = new TAreas
            {
                Id = "12341",
                CityName = "北京市",
                ParentCode = "10010",
                Deep = 1
            };
            var props = source.CheckProps(target, reset: true, setValue: true);
            Print(props.Select(t => t.Name));
            Print(target);
        }

        [TestMethod]
        public async Task TaskTest()
        {

            var t = Task.Run(() =>
            {
                throw new BusiException("busi msg");
                return "shay";
            });
            try
            {
                var w1 = t.SyncRun();
                Print(w1);
            }
            catch (Exception ex)
            {
                //BusiException
                Console.WriteLine(ex);
            }

            try
            {
                var w2 = t.Result;
                Print(w2);
            }
            catch (Exception e)
            {
                //AggregateException
                Console.WriteLine(e);
            }

            //Resolve<IUnitOfWork>().Trans(async () =>
            //{
            //    await Task.FromResult(0);
            //});

            var act = new Action(() => Console.WriteLine("shay"));
            await act;
        }

        [TestMethod]
        public void DateTableTest()
        {

            var dto = new List<DemoDto>
            {
                new DemoDto
                {
                    Id = IdentityHelper.Guid32,
                    Name = "shay",
                    Demo = DemoEnums.Demo,
                    Time = Clock.Now
                }
            };
            var result = CodeTimer.Time("datatable", 200000, () =>
            {
                var dt = dto.ToDataTable();
                var count = dt.Rows.Count;
            });
            Print(result.ToString());
        }

        [TestMethod]
        public async Task ZipTest()
        {
            var list = new List<dynamic>();
            for (var i = 0; i < 1000; i++)
            {
                list.Add(new
                { lemmaId = i.ToString(), pic = "图片问题" + i, title = $"这是第{i}条数据", url = "http://www.baidu.com" });
            }

            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
            ////进行压缩数据
            //var responseStream = new MemoryStream();
            //using (var compressedStream = new GZipStream(responseStream, CompressionMode.Compress, true))
            //{
            //    var st = new MemoryStream(oriBuffer);
            //    Console.WriteLine($"原始数据长度为={st.Length}" + "\r\n\r\n");
            //    byte[] buffer = new byte[st.Length];
            //    int checkCounter = await st.ReadAsync(buffer, 0, buffer.Length);
            //    if (checkCounter != buffer.Length) throw new ApplicationException();
            //    await compressedStream.WriteAsync(buffer, 0, buffer.Length);
            //}
            //responseStream.Position = 0;
            //var kk = responseStream;
            //var txt = Encoding.UTF8.GetString(kk.ToArray());
            //Console.WriteLine($"压缩后数据长度为={responseStream.Length},压缩结果为={txt}" + "\r\n\r\n");

            ////解压数据
            //var source = new MemoryStream();
            //using (var gs = new GZipStream(kk, CompressionMode.Decompress, true))
            //{
            //    //从压缩流中读出所有数据
            //    var bytes = new byte[4096];
            //    int n;
            //    while ((n = await gs.ReadAsync(bytes, 0, bytes.Length)) != 0)
            //    {
            //        await source.WriteAsync(bytes, 0, n);
            //    }
            //}
            //var txt1 = Encoding.UTF8.GetString(source.ToArray());
            //Console.WriteLine($"解压后数据长度为={source.Length},压缩结果为={txt1}" + "\r\n\r\n");
            //var obj = new
            //{
            //    a = "aaaa",
            //    b = 1205,
            //    c = DateTime.Now
            //};
            //var json = obj.ToJson();
            //var buffer = Encoding.UTF8.GetBytes(json);
            Print($"ori length:{buffer.Length}");
            var zip = await buffer.Zip();
            Print($"zip lenght:{zip.Length}");
            buffer = await zip.UnZip();
            var text = Encoding.UTF8.GetString(buffer);
            Print(text);
        }
    }
}
