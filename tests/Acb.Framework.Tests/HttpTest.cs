using Acb.Core.Helper.Http;
using Acb.Core.Tests;
using Acb.Core.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class HttpTest : DTest
    {
        private static async Task<dynamic> GetData()
        {
            const string url = "http://open.i-cbao.com/sys/feedback-list";
            var data = new
            {
                account = string.Empty,
                startcreatetime = string.Empty,
                endcreatetime = string.Empty,
                mobile = string.Empty,
                token =
                "ab19c595665bb1e50e1a7c1430f34a54 % 3acf74367d9c3368da5b9475dff2b4390059f57c2b6a55416ce02b39d75b27efd730331df6df684c8510e7320de8e92e18572dc06ba3f350acee6d84c055cabcd06485f0155d7381d18cb091bd4a840d133ceef9f84d6fec964b572c1c14d4e7a264d632041f417072ea853ea5d2c7cb60a271ced7b340e1bb953ae838246ca3c8bfeb3ef97091d5a38316f1259ef6b8b74601be57d552ba51cea67b5f71d1da13",
                ticket =
                "ab19c595665bb1e50e1a7c1430f34a54 % 3acf74367d9c3368da5b9475dff2b4390059f57c2b6a55416ce02b39d75b27efd730331df6df684c8510e7320de8e92e18572dc06ba3f350acee6d84c055cabcd06485f0155d7381d18cb091bd4a840d133ceef9f84d6fec964b572c1c14d4e7a264d632041f417072ea853ea5d2c7cb60a271ced7b340e1bb953ae838246ca3c8bfeb3ef97091d5a38316f1259ef6b8b74601be57d552ba51cea67b5f71d1da13",
                status = -1,
                index = 1,
                size = 13
            };
            var rest = new RestHelper();
            return await rest.PostAsync(url, data);
        }

        [TestMethod]
        public void RestHelperTest()
        {
            var time = TimeSpan.FromDays(7) - TimeSpan.FromSeconds(562382);
            var loginTime = Clock.Now.AddSeconds(-time.TotalSeconds);
            Print($"{loginTime:yyyy-MM-dd HH:mm:ss}");
            //const string uri = "/query/life?t=1";
            //var helper = new RestHelper(HelperTest.Site.Market);
            //var result = await helper.GetAsync<DResult<dynamic>>(uri, new
            //{
            //    cityCode = "510100"
            //});
            //var xx = JsonConvert.SerializeObject(html.Data.xianXing);
            //if (html.Data.xianxing != null)
            //Print(result);
        }

        [TestMethod]
        public async Task TaskTest()
        {
            var result = await CodeTimer.Time("http", 10, async () =>
            {
                var rest = new RestHelper("http://192.168.0.231:8889");
                var config = await rest.GetAsync("basic/dev");
                Print(config);
            }, 5);
            Print(result.ToString());
        }
    }
}
