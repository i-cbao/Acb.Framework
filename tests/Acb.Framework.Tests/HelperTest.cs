using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acb.Core.Extensions;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class HelperTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<HelperTest>();

        [TestMethod]
        public void Md5Test()
        {
            Print("L7iBVV52RCs0IuWcvrnSRwQpVqOyz9y3KFONd5ZhBc5/ev84j27711veJ/Pj82UveRsV4zq7EcN7H+JABWBZBy71agOv2xDB3Oous/1TSCQwax3CBcaWliooj78Z037JeYsXd4BM2HCihhUsEwXvJHgHJFfCLF/f3yEMyKRvEL+MKLyW+CBD/tKdaxJMkZ2fZGoVDR2+K7QlNNmX704T6mFg5nlxKXny4mwHz7taYz6TQgzrvRpkAooV65VPN32uA5XDibmQaAo=".UrlEncode());
            return;
            var str = "20180427144321".Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Insert(16, ":");
            Print(str);
            var time = DateTime.Parse(str);
            Print(time);
            //Print("1523879631960" + EncryptHelper.MD5("account=1&password=12385653b8832ad55cd1523879631960"));
            Print(IdentityHelper.Guid16);
            //Print(Utils.GetSpellCode("±±¾©"));
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
        public async Task HttpTest()
        {
            const string uri = "/query/life?t=1";
            var helper = new RestHelper(Site.Market);
            var result = await helper.GetAsync<DResult<dynamic>>(uri, new
            {
                cityCode = "510100"
            });
            //var xx = JsonConvert.SerializeObject(html.Data.xianXing);
            //if (html.Data.xianxing != null)
            Print(result);
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
    }
}
