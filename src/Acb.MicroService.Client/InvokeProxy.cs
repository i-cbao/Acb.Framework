using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.MicroService.Client.ServiceFind;
using Acb.MicroService.Client.ServiceFinder;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Acb.MicroService.Client
{
    /// <inheritdoc />
    /// <summary> 代理调用 </summary>
    /// <typeparam name="T"></typeparam>
    public class InvokeProxy<T> : DispatchProxy where T : IMicroService
    {
        private readonly ILogger _logger = LogManager.Logger(typeof(ProxyService));
        private readonly MicroServiceConfig _config;


        /// <summary> 接口类型 </summary>
        private readonly Type _type;

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        public InvokeProxy()
        {
            _type = typeof(T);
            _config = Constans.MicroSreviceKey.Config<MicroServiceConfig>();
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

        private List<string> GetTypeService()
        {
            var finder = GetServiceFinder();
            var urls = finder.Find(_type.Assembly, _config);

            if (urls == null || !urls.Any())
                throw new BusiException($"{_type.FullName},没有可用的服务");
            return urls.Select(url => $"{url}{_type.Name}/").ToList();
        }

        /// <inheritdoc />
        /// <summary> 接口调用 </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var services = GetTypeService();
            var service = string.Empty;

            var builder = Policy
                .Handle<AggregateException>(ex => ex.GetBaseException() is HttpRequestException) //服务器异常
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound); //服务未找到
            //熔断,3次异常,熔断5分钟
            var breaker = builder.CircuitBreaker(3, TimeSpan.FromMinutes(5));
            //重试3次
            var retry = builder.Retry(3, (result, count) =>
            {
                _logger.Warn(result.Exception != null
                    ? $"{service}:retry,{count},{result.Exception.Format()}"
                    : $"{service}:retry,{count},{result.Result.StatusCode}");
                services.Remove(service);
            });

            var policy = Policy.Wrap(retry, breaker);

            var resp = policy.Execute(() =>
            {
                if (!services.Any())
                    throw ErrorCodes.NoService.CodeException();
                service = services.RandomSort().First();
                var url = string.Concat(service, targetMethod.Name);
                var remoteIp = AcbHttpContext.RemoteIpAddress;
                var headers = new Dictionary<string, string>
                {
                    {"X-Forwarded-For", remoteIp},
                    {"X-Real-IP", remoteIp},
                    {"User-Agent", AcbHttpContext.Current == null ? "micro_service_client" : AcbHttpContext.UserAgent}
                };
                //http请求
                return HttpHelper.Instance.RequestAsync(HttpMethod.Post, new HttpRequest(url)
                {
                    Data = args,
                    Headers = headers
                }).Result;
            });

            //if (!services.Any())
            //    throw ErrorCodes.NoService.CodeException();
            //service = services.RandomSort().First();
            //var url = string.Concat(service, targetMethod.Name);
            ////http请求
            //var resp = HttpHelper.Instance.RequestAsync(HttpMethod.Post, url, data: args).Result;

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var html = resp.Content.ReadAsStringAsync().Result;
                var type = targetMethod.ReturnType;
                return JsonConvert.DeserializeObject(html, type);
            }
            else
            {
                var html = resp.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<DResult>(html);
                throw new BusiException(result.Message, result.Code);
            }
        }
    }
}
