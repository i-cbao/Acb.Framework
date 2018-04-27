using System.Threading.Tasks;
using Acb.ApiClient;
using Acb.ApiClient.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ApiClientTest : DTest
    {
        public interface IMyClass : IHttpApiClient
        {
            [HttpGet("")]
            ITask<string> GetHtmlAsync([Url]string url);
        }

        [TestMethod]
        public async Task ClientTest()
        {
            var client = ApiClient.HttpApiClient.Create<IMyClass>();
            var html = await client.GetHtmlAsync("http://www.baidu.com");
            Print(html);
        }
    }
}
