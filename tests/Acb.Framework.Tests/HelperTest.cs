using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Core.Tests;
using Acb.Core.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class HelperTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<HelperTest>();

        [TestMethod]
        public void Md5Test()
        {
            const int count = 20;
            var ev = new CountdownEvent(count);
            var teacher = new Task(() =>
            {
                Print($"{Clock.Now:yyyy-MM-dd HH:mm:ss},发卷子");
                ev.Wait();
                Print($"{Clock.Now:yyyy-MM-dd HH:mm:ss},收卷子");
            });
            teacher.Start();
            Thread.Sleep(1000);
            var students = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                var student = new Task(state =>
                    {
                        var time = Clock.Now;
                        Print($"{time:yyyy-MM-dd HH:mm:ss},学生{state}，开始作答");
                        Thread.Sleep(RandomHelper.Random().Next(5, 12) * 1000);
                        Print(
                            $"{Clock.Now:yyyy-MM-dd HH:mm:ss},学生{state}，交卷,耗时：{(Clock.Now - time).TotalMilliseconds}ms");
                        ev.Signal();
                    }, i + 1);
                student.Start();
                students.Add(student);
            }

            Task.WaitAll(students.Union(new[] { teacher }).ToArray());
            //var t = new { a = "abc", b = "123" };
            //var t = new[] { "abc", "123" };
            //t.Each(Print);
            //var sign = $"TEST0519011500001454acbtest".Md5();
            //Print(sign);
            ////var str = "20180427144321".Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Insert(16, ":");
            //Print(str);
            //var time = DateTime.Parse(str);
            //Print(time);
            //Print("1523879631960" + EncryptHelper.MD5("account=1&password=12385653b8832ad55cd1523879631960"));
            //Print(IdentityHelper.Guid16);
            //Print(Utils.GetSpellCode("北京"));
            //Print(Clock.Now.ToTimestamp());
            //var md5 = "shay".Md5();
            //_logger.Info(md5);
            //Assert.AreEqual(md5.Length, 32);
        }

        [TestMethod]
        public async Task ImageTest()
        {
            const string uri =
                "http://mmbiz.qpic.cn/mmbiz_jpg/z7rLsAcq7eiaAuSMhfydftfVkv71LrhaquK0WTtaqQQYK42mQDTMCBjvluQYZxE9KNVJk9Vyk5FFlBfwPEPz6tw/640?wx_fmt=jpeg&tp=webp&wxfrom=5&wx_lazy=1";
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(uri);
            using (var fs = new FileStream("d:\\test.jpg", FileMode.Create))
            {
                //stream.Seek(0, SeekOrigin.Begin);
                var bArr = new byte[1024];
                var size = stream.Read(bArr, 0, bArr.Length);
                while (size > 0)
                {
                    fs.Write(bArr, 0, size);
                    size = stream.Read(bArr, 0, bArr.Length);
                }

                stream.Close();
                fs.Flush();
            }
        }

        public enum Site
        {
            User,
            Story,
            Payment,
            Market
        }

        [TestMethod]
        public async Task RestHelperTest()
        {
            const string uri = "/api/home/logLevel";
            var helper = new RestHelper("http://localhost:61487");
            var result = await helper.GetAsync<DResult<dynamic>>(uri);
            Print(result);
        }

        [TestMethod]
        public async Task Test_01()
        {
            const string uri = "https://auto.sinosafe.com.cn/DoubleCodeinput/query?_t={0}";
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("precondition","companyCode,businessMode,,agreementNo"),
                new KeyValuePair<string, string>("preconditionValue","0110060101,2,,B0110100106"),
                new KeyValuePair<string, string>("manySelect","1"),
                new KeyValuePair<string, string>("hideValue",""),
                new KeyValuePair<string, string>("otherCondition",""),
                new KeyValuePair<string, string>("rowsPerPage","5"),
                new KeyValuePair<string, string>("pageNo","0"),
                new KeyValuePair<string, string>("fieldValue",""),
                new KeyValuePair<string, string>("codeType","getSalesmanCode")
            });
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, string.Format(uri, DateTime.Now.Ticks));
            req.Headers.Add("Referer", "https://auto.sinosafe.com.cn/");
            req.Headers.Add("Cookie",
                "aliyungf_tc=AQAAAPGZzQyavQwAWsXWqwl1FQYwVSBH; corevins80=At2tZwsEAgr0sPgKPUMjRg$$; JSESSIONID=fY_benM9P1Eici6X74D-qh8sjuj9O8aKjopY1heAkWF6XTYznpfh!-83576508; menu_cookie=%2Fquotation%2Fview");
            req.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            req.Content = data;
            var resp = await client.SendAsync(req);
            var html = await resp.Content.ReadAsStringAsync();
            Print(html);
            //var headers = new Dictionary<string, string>
            //{
            //    {"Referer", "https://auto.sinosafe.com.cn/"},
            //    {
            //        "Cookie",
            //        "aliyungf_tc=AQAAAPGZzQyavQwAWsXWqwl1FQYwVSBH; corevins80=At2tZwsEAgr0sPgKPUMjRg$$; JSESSIONID=fY_benM9P1Eici6X74D-qh8sjuj9O8aKjopY1heAkWF6XTYznpfh!-83576508; menu_cookie=%2Fquotation%2Fview"
            //    },
            //    {"ContentType", "application/x-www-form-urlencoded"}
            //};
            //var resp = await HttpHelper.Instance.RequestAsync(HttpMethod.Post,
            //    string.Format(uri, Clock.Now.ToTimestamp()), headers: headers, content: data);
            //var html = await resp.Content.ReadAsStringAsync();
            //Print(html);
        }

        [TestMethod]
        public async Task CspTest()
        {
            var htmlCollection = new BlockingCollection<string>();
            var client = new HttpClient();
            var urls = new ConcurrentQueue<string>(new[]
            {
                "https://www.github.com",
                "http://www.baidu.com",
                "https://www.cnblogs.com/",
                "http://www.csdn.net"
            });
            //produce
            {
                await Task.Factory.StartNew(async () =>
                {
                    var tasks = urls.Select(async url =>
                    {
                        var html = await client.GetStringAsync(url);
                        htmlCollection.Add(html);
                    }).ToArray();
                    await Task.WhenAll(tasks);
                    htmlCollection.CompleteAdding();
                });
            }
            //consume
            {
                foreach (var html in htmlCollection.GetConsumingEnumerable())
                {
                    var title = RegexHelper.Match(html, "<title>(.+)</title>", RegexOptions.IgnoreCase);
                    Console.WriteLine(title);
                }
            }
        }

        private async Task RunTask()
        {
            Print("run start");
            Print($"thread: {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(TimeSpan.FromSeconds(5));
            Print("run complete");
        }

        [TestMethod]
        public async Task TaskTest()
        {
            var watcher = Stopwatch.StartNew();
            Print($"thread: {Thread.CurrentThread.ManagedThreadId}");
            await RunTask();
            Print($"thread: {Thread.CurrentThread.ManagedThreadId}");
            watcher.Stop();
            Print($"await 耗时:{watcher.ElapsedMilliseconds}ms");
            watcher.Restart();
            RunTask().SyncRun();
            Print($"thread: {Thread.CurrentThread.ManagedThreadId}");
            watcher.Stop();
            Print($"run 耗时:{watcher.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void LongIdTest()
        {
            var result = CodeTimer.Time("long id", 5, () =>
            {
                var id = IdentityHelper.LongId;
                Print(id);
            });
            Print(result.ToString());
        }

        [TestMethod]
        public async Task NotifyTest()
        {
            const string host = "http://localhost:25859";
            //const string host = "https://pay.i-cbao.com";
            var resp = await HttpHelper.Instance.PostAsync(new HttpRequest($"{host}/notify")
            {
                Data =
                    "gmt_create=2019-07-12+18%3A53%3A07&charset=UTF-8&seller_email=ichebao%40i-cbao.com&subject=I%E8%BD%A6%E4%BF%9D%E7%BB%AD%E4%BF%9D%E8%AE%A2%E5%8D%95%E6%94%AF%E4%BB%98&sign=Jk0ri1d8W4hs%2B3MdT%2BlxZ%2FLLOvTyU7Lb5wDjtTBE53iPmcuue8s45qcFg7pJpcGtjkl8PdQ%2FrnKGKNZgJFFOr9iFZOhwudTYe3A6HN2h6zQzHKS9cPltSwaHXClMSKLOXFCyFFc%2B4n9H%2BrO%2BXW6s6bipKYdRyTlBa1yFZvl2yhcDOUyvw9bwbZmaLsIMHA9myfIVIwjN96Rere49TR4FjcQeeyPmIGm27lmOuc9dnXH9dusIErmZ6u0MY7nWbHJwCN3ddTU67%2BPHcu%2BKs4RbBQV4YaKPAhd4oxIR4Zsq%2BVnRUD%2FBMtDNpuDVhvPeuNlP08kqGmpAn6fvUE2RsjVTtA%3D%3D&body=I%E8%BD%A6%E4%BF%9D%E7%BB%AD%E4%BF%9D%E8%AE%A2%E5%8D%95%5BRO1562928747020%5D%E5%9C%A8%E7%BA%BF%E6%94%AF%E4%BB%98&buyer_id=2088002241808465&invoice_amount=0.01&notify_id=2019071200222185307008461040191282&fund_bill_list=%5B%7B%22amount%22%3A%220.01%22%2C%22fundChannel%22%3A%22PCREDIT%22%7D%5D&notify_type=trade_status_sync&trade_status=TRADE_SUCCESS&receipt_amount=0.01&app_id=2017102609532728&buyer_pay_amount=0.01&sign_type=RSA2&seller_id=2088621977822732&gmt_payment=2019-07-12+18%3A53%3A07&notify_time=2019-07-12+19%3A18%3A11&version=1.0&out_trade_no=T246055688930034411&total_amount=0.01&trade_no=2019071222001408461048341241&auth_app_id=2017102609532728&buyer_logon_id=luo***%40163.com&point_amount=0.00",
                BodyType = HttpBodyType.Form
            });
            var content = await resp.Content.ReadAsStringAsync();
            Print(content);
        }

        [TestMethod]
        public void CommandTest()
        {
            Utils.ExecCommand(cmd =>
            {
                cmd("ping www.baidu.com");
                cmd("ping www.i-cbao.com");
            }, Print, 8000);
        }
    }
}
