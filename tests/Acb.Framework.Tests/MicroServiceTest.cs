﻿using Acb.Core.Data;
using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Core.Tests;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.Enums;
using Acb.MicroService.Client;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

        protected override void MapServices(IServiceCollection services)
        {
            services.AddMicroClient();
            base.MapServices(services);
        }

        public MicroServiceTest()
        {
            _microService = ProxyService.Proxy<IDemoService>();
            _localService = CurrentIocManager.Resolve<IDemoService>();
        }

        [TestMethod]
        public async Task LoadTest()
        {
            _microService.Load("shay");
            await _microService.LoadAsync();
            var dict = await _microService.Dict(new[] { "123", "456" });
            Print(dict);
        }

        [TestMethod]
        public void Test()
        {

            var logger = LogManager.Logger<MicroServiceTest>();
            var localResult = CodeTimer.Time("local", 1, async () =>
             {
                 try
                 {
                     var dict = await _localService.Areas("510100");
                     Print(dict);
                 }
                 catch (Exception ex)
                 {
                     logger.Error(ex.Message, ex);
                     throw;
                 }
             }, 1);
            //var result = CodeTimer.Time("micro", 100, async () =>
            //{
            //    try
            //    {
            //        var dict = await _microService.Areas("510100");
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error(ex.Message, ex);
            //        throw;
            //    }
            //}, 10);
            Print(localResult.ToString());
            //Print(result.ToString());

            Print(CurrentIocManager.Resolve<IDbConnectionProvider>().ToString());
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
                var str = (data.GetType().IsSimpleType() || data.GetType().IsEnum)
                    ? data.ToString()
                    : JsonHelper.ToJson(data);
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
            var data = new object[]
            {
                IdentityHelper.Guid32,
                new DemoInputDto
                {
                    Demo = DemoEnums.Test,
                    Name = "shay" + IdentityHelper.Guid16
                }
            };

            var helper = HttpHelper.Instance;
            Task.Run(async () =>
            {
                var req = await helper.PostAsync(url, data);
                var html = await req.Content.ReadAsStringAsync();
                Print(html);
                html = await PostAsync(url, data);
                Print(html);
            }).Wait();

            LogManager.LogLevel(LogLevel.Off);
            var resul2 = CodeTimer.Time("WebRequest", 20, async () =>
            {
                var html = await PostAsync(url, data);
                Print($"web_request:{html}");
            }, 10);
            var result1 = CodeTimer.Time("HttpClient", 20, async () =>
            {
                var req = await helper.PostAsync(url, data);
                var html = await req.Content.ReadAsStringAsync();
                Print($"http_client:{html}");
            }, 10);
            Print(result1.ToString());
            Print(resul2.ToString());
        }

        [TestMethod]
        public void JsonTest()
        {
            var service = Bootstrap.Container.ResolveKeyed<IDemoService>("proxy"); //(IDemoService)ProxyService.Proxy(typeof(IDemoService));
            Print(service.Areas("1").Result);
            //const string json = "[510100]";
            //var array = JsonConvert.DeserializeObject<JArray>(json);
            //var value = array[0].Value<string>().CastTo(typeof(int));
            //Print(value);
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
