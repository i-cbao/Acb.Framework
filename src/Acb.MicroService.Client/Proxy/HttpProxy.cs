using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.MicroService.Client.ServiceFinder;
using Newtonsoft.Json;
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
    /// <summary> Http代理调用 </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpProxy<T> : ProxyAsync where T : IMicroService
    {
        private readonly ILogger _logger = LogManager.Logger(typeof(HttpProxy<>));
        private readonly MicroServiceConfig _config;
        private readonly ICache _serviceCache;

        /// <summary> 接口类型 </summary>
        private readonly Type _type;

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        public HttpProxy()
        {
            _type = typeof(T);
            _config = Constans.MicroSreviceKey.Config<MicroServiceConfig>();
            _serviceCache = CacheManager.GetCacher(typeof(HttpProxy<>));
        }

        private IServiceFinder GetServiceFinder()
        {
            switch (_config.Register)
            {
                case RegisterType.Consul:
                    return new ConsulServiceFinder();
                case RegisterType.Redis:
                    return new RedisServiceFinder();
                default:
                    return new RedisServiceFinder();
            }
        }

        private IEnumerable<string> GetService()
        {
            var finder = GetServiceFinder();
            var urls = finder.Find(_type.Assembly, _config).ToList();
            if (urls == null || !urls.Any())
                throw ErrorCodes.NoService.CodeException();
            return urls;
        }

        private List<string> GetTypeService()
        {
            var services = GetService();
            return services.Select(url => new Uri(new Uri(url), $"micro/{_type.Name}/").AbsoluteUri).ToList();
        }

        private Task<object> BaseInvoke(MethodInfo targetMethod, IEnumerable args)
        {
            var services = GetTypeService();
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
            });

            var policy = Policy.WrapAsync(retry, breaker);

            var resp = policy.ExecuteAsync(async () =>
            {
                if (!services.Any())
                {
                    _serviceCache.Remove(_type.Assembly.AssemblyKey());
                    throw ErrorCodes.NoService.CodeException();
                }
                service = services.First();
                var url = string.Concat(service, targetMethod.Name);
                return await InvokeAsync(url, args);
            });
            var type = targetMethod.ReturnType;
            if (type == typeof(void) || type == typeof(Task))
                return Task.FromResult<object>(0);
            return ResultAsync(resp, type);
        }

        /// <inheritdoc />
        /// <summary> 接口调用 </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return BaseInvoke(targetMethod, args).GetAwaiter().GetResult();
        }

        public override Task InvokeAsync(MethodInfo method, object[] args)
        {
            return BaseInvoke(method, args);
        }

        public override async Task<TR> InvokeAsyncT<TR>(MethodInfo method, object[] args)
        {
            var result = await BaseInvoke(method, args);
            return result.CastTo<TR>();
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
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var html = await resp.Content.ReadAsStringAsync();
                if (typeof(Task<>).IsGenericAssignableFrom(returnType))
                {
                    returnType = returnType.GenericTypeArguments[0];
                }
                return JsonConvert.DeserializeObject(html, returnType);
            }
            else
            {
                var html = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<DResult>(html);
                throw new BusiException(result.Message, result.Code);
            }
        }
    }
}
