using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Redis;
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
        private const string MicroSreviceKey = "micro_service";
        private const string RegistCenterKey = MicroSreviceKey + ":center";
        private readonly ILogger _logger = LogManager.Logger(typeof(ProxyService));

        private string RedisKey
        {
            get
            {
                var key = $"{MicroSreviceKey}:redisKey".Config<string>();
                return string.IsNullOrWhiteSpace(key) ? RegistCenterKey : key;
            }
        }


        /// <summary> 接口类型 </summary>
        private readonly Type _type;

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        public InvokeProxy()
        {
            _type = typeof(T);
        }

        private List<string> GetTypeService()
        {
            var redis = RedisManager.Instance.GetDatabase();
            var assemblyKey = _type.Assembly.AssemblyKey();
            var urls = redis.SetMembers($"{RedisKey}:{assemblyKey}");
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
                    {"X-Real-IP", remoteIp}
                };
                //http请求
                return HttpHelper.Instance.RequestAsync(HttpMethod.Post, url, data: args, headers: headers).Result;
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
