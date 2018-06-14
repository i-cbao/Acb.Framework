using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Core.Tests;
using Acb.Core.Timing;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.Enums;
using Acb.MicroService.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class MicroServiceTest : DTest
    {
        private readonly IDemoService _microService;
        private readonly IDemoService _localService;

        public MicroServiceTest()
        {
            _microService = ProxyService.Proxy<IDemoService>();
            _localService = CurrentIocManager.Resolve<IDemoService>();
        }

        [TestMethod]
        public void Test()
        {

            _microService.Load("shay");
            var task = _microService.LoadAsync();
            var dict = _microService.Dict(new[] { "123", "456" });
            Print(dict);

            //LogManager.LogLevel(LogLevel.Off);
            //var result = CodeTimer.Time("micro", 200, () =>
            //{
            //    _microService.Hello(IdentityHelper.Guid32, new DemoInputDto
            //    {
            //        Demo = DemoEnums.Test,
            //        Name = "shay" + IdentityHelper.Guid16,
            //        Time = Clock.Now
            //    });
            //    //Print(word);
            //}, 10);
            //Print(result.ToString());
            //result = CodeTimer.Time("local", 200, () =>
            //{
            //    _localService.Hello(IdentityHelper.Guid32, new DemoInputDto
            //    {
            //        Demo = DemoEnums.Test,
            //        Name = "shay" + IdentityHelper.Guid16,
            //        Time = Clock.Now
            //    });
            //    //Print(word);
            //}, 10);
            //Print(result.ToString());
        }

        private async Task<WebResponse> Request(HttpMethod method, string url, object data = null)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method.ToString();
            req.Accept = "application/json";
            if (data == null || method == HttpMethod.Get || method == HttpMethod.Delete)
                return await req.GetResponseAsync();
            req.ContentType = "application/json";
            using (var stream = await req.GetRequestStreamAsync())
            {
                var str = data.GetType().IsSimpleType() || data.GetType().IsEnum ? data.ToJson() : data.ToString();
                var buffer = Encoding.UTF8.GetBytes(str);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                req.ContentLength = buffer.Length;
            }

            return await req.GetResponseAsync();
        }

        private async Task<string> PostAsync(string url, object data)
        {
            var stream = await PostToStreamAsync(url, data);
            if (stream == null) return string.Empty;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<Stream> PostToStreamAsync(string url, object data)
        {
            var resp = await Request(HttpMethod.Post, url, data);
            return resp.GetResponseStream();
        }

        [TestMethod]
        public void HttpTest()
        {
            const string url = "http://localhost:63409/micro/idemoservice/hello";
            var data = JsonHelper.ToJson(new object[]
            {
                IdentityHelper.Guid32,
                new DemoInputDto
                {
                    Demo = DemoEnums.Test,
                    Name = "shay" + IdentityHelper.Guid16,
                    Time = Clock.Now
                }
            });

            LogManager.LogLevel(LogLevel.Off);
            var result1 = CodeTimer.Time("HttpClient", 200, () =>
            {
                var html = new HttpClient().PostAsync(url,
                        new StringContent(data, Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync()
                    .Result;
            }, 10);
            var resul2 = CodeTimer.Time("WebRequest", 200, () =>
            {
                var html = PostAsync(url, data).Result;
            }, 10);
            Print(result1.ToString());
            Print(resul2.ToString());
        }

        [TestMethod]
        public void JsonTest()
        {
            const string json = "[510100]";
            var array = JsonConvert.DeserializeObject<JArray>(json);
            var value = array[0].Value<string>().CastTo(typeof(int));
            Print(value);
        }
        //[TestMethod]
        //public void LifeServiceTest()
        //{
        //    var dtos = _marketContract.GetLifeService("510100");
        //    Print(dtos);
        //    Assert.AreNotEqual(dtos, null);
        //}

        //[TestMethod]
        //public void VehiceIllegalsTest()
        //{
        //    const string vehiceId = "648fc3fac84641f69a8c2dd6c2306f84";
        //    var dtos = _marketContract.GetVehiceIllegals(vehiceId, 1, 10);
        //    Print(dtos);
        //    Assert.AreNotEqual(dtos, null);
        //}

    }
}
