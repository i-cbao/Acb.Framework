using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
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
            var md5 = "shay".Md5();
            _logger.Info(md5);
            Assert.AreEqual(md5.Length, 32);
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
            var html = await helper.GetAsync<DResult<dynamic>>(uri, new
            {
                cityCode = "510100"
            });
            //var xx = JsonConvert.SerializeObject(html.Data.xianXing);
            //if (html.Data.xianxing != null)
            Print(html.Data.xianXing);
        }
    }
}
