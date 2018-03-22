using System;
using System.Net.Http;
using System.Reflection;
using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Redis;
using Newtonsoft.Json;

namespace Acb.MicroService.Client
{
    /// <summary> 代理调用 </summary>
    /// <typeparam name="T"></typeparam>
    public class InvokeProxy<T> : DispatchProxy where T : IMicroService
    {
        /// <summary> 接口类型 </summary>
        private Type type = null;

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        public InvokeProxy()
        {
            type = typeof(T);
        }

        private string GetTypeService()
        {
            var redis = RedisManager.Instance.GetDatabase(defaultDb: 2);
            var url = redis.HashGet(Constants.RegistCenterKey, type.FullName);
            return $"{url}{type.Name}/";
        }

        /// <inheritdoc />
        /// <summary> 接口调用 </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine($"Invoke 远程服务调用！{targetMethod.Name}");
            Console.WriteLine(JsonHelper.ToJson(args));

            var data = args.Length == 0 ? null : args[0];
            var url = $"{GetTypeService()}{targetMethod.Name}";
            var resp = HttpHelper.Instance
                .RequestAsync(HttpMethod.Post, url, data: data).Result;
            var html = resp.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject(html, targetMethod.ReturnType);
        }
    }

    public class ProxyService
    {
        /// <summary> 生成代理 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Proxy<T>() where T : IMicroService
        {
            return DispatchProxy.Create<T, InvokeProxy<T>>();
        }
    }
}
