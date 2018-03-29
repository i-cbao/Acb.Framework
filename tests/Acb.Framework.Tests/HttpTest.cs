using Acb.Core.Helper;
using Acb.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class HttpTest : DTest
    {
        private static async Task<HttpResponseMessage> GetData()
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
            return await HttpHelper.Instance.RequestAsync(HttpMethod.Post, url, data: data);
        }

        [TestMethod]
        public void TaskTest()
        {
            var result = CodeTimer.Time("test", 200, () =>
            {
                var resp = GetData().Result;
                var html = resp.Content.ReadAsStringAsync().Result;
            }, 10);
            Print(result.ToString());
        }
    }
}
