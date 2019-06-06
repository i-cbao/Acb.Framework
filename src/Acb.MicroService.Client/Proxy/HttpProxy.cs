using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Core.Message;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.Proxy
{
    public class HttpProxy<T> : HttpProxy where T : IMicroService
    {
        public HttpProxy() : base(typeof(T))
        {
        }
    }

    /// <summary> Http代理调用 </summary>
    public class HttpProxy : ProxyAsync
    {
        private readonly ILogger _logger = LogManager.Logger(typeof(HttpProxy));
        private readonly IServiceRouter _serviceRouter;

        /// <summary> 接口类型 </summary>
        private readonly Type _type;

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        public HttpProxy(Type type)
        {
            _type = type;
            _serviceRouter = CurrentIocManager.Resolve<IServiceRouter>();
        }

        private async Task<IEnumerable<string>> GetService()
        {
            var urls = (await _serviceRouter.Find(_type)).ToList();
            if (urls == null || !urls.Any())
                throw ErrorCodes.NoService.CodeException();
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
                _logger.Warn(result.Exception != null
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
                    throw ErrorCodes.NoService.CodeException();
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
        private static async Task<HttpResponseMessage> InvokeAsync(string url, IEnumerable args)
        {
            var remoteIp = AcbHttpContext.RemoteIpAddress;
            var headers = new Dictionary<string, string>
            {
                {"X-Forwarded-For", remoteIp},
                {"X-Real-IP", remoteIp},
                {
                    "User-Agent", AcbHttpContext.Current == null ? "micro_service_client" : AcbHttpContext.UserAgent
                },
                {"referer", AcbHttpContext.RawUrl}
            };
            return await HttpHelper.Instance.RequestAsync(HttpMethod.Post, new HttpRequest(url)
            {
                Data = args,
                Headers = headers
            });
        }

        /// <summary> 获取结果 </summary>
        /// <param name="respTask"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        private static async Task<object> ResultAsync(Task<HttpResponseMessage> respTask, Type returnType)
        {
            var resp = await respTask;
            var codec = CurrentIocManager.Resolve<IMessageCodec>();
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var data = await resp.Content.ReadAsByteArrayAsync();
                return codec.Decode(data, returnType);
            }
            else
            {
                var data = await resp.Content.ReadAsByteArrayAsync();
                var result = codec.Decode<DResult>(data);
                throw new BusiException(result.Message, result.Code);
            }
        }
    }
}
