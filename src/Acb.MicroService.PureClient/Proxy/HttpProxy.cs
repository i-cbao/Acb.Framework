using Acb.MicroService.PureClient.Router;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Acb.MicroService.PureClient.Proxy
{
    public class HttpProxy<T> : HttpProxy
    {
        public HttpProxy() : base(typeof(T))
        {
        }
    }

    /// <summary> Http代理调用 </summary>
    public class HttpProxy : ProxyAsync
    {
        private readonly ILogger _logger;
        private readonly IRouter _serviceRouter;

        private readonly IMessageCodec _messageCodec;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary> 接口类型 </summary>
        private readonly Type _type;

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        public HttpProxy(Type type)
        {
            _type = type;
            _logger = Extensions.GetService<ILogger<HttpProxy>>();
            _serviceRouter = Extensions.GetService<IRouter>();
            _messageCodec = Extensions.GetService<IMessageCodec>();
            _httpClientFactory = Extensions.GetService<IHttpClientFactory>();
            _contextAccessor = Extensions.GetService<IHttpContextAccessor>();
        }

        private async Task<IEnumerable<string>> GetService()
        {
            var urls = await _serviceRouter.Find(_type);
            if (urls == null || !urls.Any())
                throw new MicroClientException("no services", 10007);
            return urls.Select(t => t.ToString());
        }

        private async Task<List<string>> GetTypeService()
        {
            var services = await GetService();
            return services.Select(url => new Uri(new Uri(url), $"micro/{_type.Name}/").AbsoluteUri).ToList();
        }

        protected override async Task<object> BasicInvokeAsync(MethodInfo targetMethod, object[] args)
        {
            var services = await GetTypeService();
            var service = string.Empty;
            var builder = Policy
                .Handle<HttpRequestException>() //服务器异常
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound); //服务未找到
            //熔断,3次异常,熔断5分钟
            var breaker = builder.CircuitBreakerAsync(3, TimeSpan.FromMinutes(5));
            //重试3次
            var retry = builder.RetryAsync(3, (result, count) =>
            {
                _logger.LogWarning(result.Exception != null
                    ? $"{service}{targetMethod.Name}:retry,{count},{result.Exception.Format()}"
                    : $"{service}{targetMethod.Name}:retry,{count},{result.Result.StatusCode}");
                services.Remove(service);
                _serviceRouter.CleanCache(_type);
            });

            var policy = Policy.WrapAsync(retry, breaker);

            var resp = policy.ExecuteAsync(async () =>
            {
                if (!services.Any())
                {
                    await _serviceRouter.CleanCache(_type);
                    throw new MicroClientException("no services", 10007);
                }
                service = services.First();
                var url = string.Concat(service, targetMethod.Name);
                return await InvokeAsync(url, args);
            });

            //获取拦截
            var returnType = GetReturnType(targetMethod);
            if (returnType == typeof(void) || returnType == typeof(Task))
                return null;
            var value = await ResultAsync(resp, returnType);

            return value;
        }

        /// <summary> 执行请求 </summary>
        /// <param name="url"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> InvokeAsync(string url, IEnumerable args)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(args);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            if (_contextAccessor?.HttpContext != null)
            {
                var context = _contextAccessor.HttpContext;
                var headers = context.Request.Headers;
                request.Headers.Add("referer", context.RawUrl());
                if (headers.TryGetValue("User-Agent", out var userAgent))
                    request.Headers.Add("User-Agent", userAgent.ToString());
                var remoteIp = context.RemoteIp();
                request.Headers.Add("X-Forwarded-For", remoteIp);
                request.Headers.Add("X-Real-IP", remoteIp);
            }
            else
            {
                request.Headers.Add("User-Agent", "micro_service_client");
                var remoteIp = Extensions.LocalIp();
                request.Headers.Add("X-Forwarded-For", remoteIp);
                request.Headers.Add("X-Real-IP", remoteIp);
            }
            return await client.SendAsync(request);
        }

        /// <summary> 获取结果 </summary>
        /// <param name="respTask"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        private async Task<object> ResultAsync(Task<HttpResponseMessage> respTask, Type returnType)
        {
            var resp = await respTask;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var data = await resp.Content.ReadAsByteArrayAsync();
                return _messageCodec.Decode(data, returnType);
            }
            else
            {
                var data = await resp.Content.ReadAsByteArrayAsync();
                var result = _messageCodec.Decode<MicroResult>(data);
                throw new MicroClientException(result.Message, result.Code);
            }
        }
    }
}
