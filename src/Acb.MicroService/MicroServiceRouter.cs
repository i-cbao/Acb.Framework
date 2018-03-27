using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Reflection;
using Acb.Core.Serialize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Acb.MicroService
{
    /// <summary> 微服务路由 </summary>
    public class MicroServiceRouter : IRouter
    {
        private static readonly ConcurrentDictionary<string, MethodInfo>
            Methods = new ConcurrentDictionary<string, MethodInfo>();

        internal static readonly HashSet<string> ServiceAssemblies = new HashSet<string>();

        /// <summary> 初始化服务 </summary>
        internal static void InitServices()
        {
            var services = CurrentIocManager.Resolve<ITypeFinder>()
                .Find(t => typeof(IMicroService).IsAssignableFrom(t) && t.IsInterface && t != typeof(IMicroService))
                .ToList();
            foreach (var service in services)
            {
                var assKey = service.Assembly.AssemblyKey();
                if (!ServiceAssemblies.Contains(assKey))
                    ServiceAssemblies.Add(assKey);
                var methods = service.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    Methods.TryAdd($"{service.Name}/{method.Name}".ToLower(), method);
                }
            }
        }

        private static async Task WriteJsonAsync(HttpContext ctx, object data, int code = (int)HttpStatusCode.OK)
        {
            var response = ctx.Response;
            response.StatusCode = code;
            response.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(data));
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary> 路由处理 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task RouteAsync(RouteContext context)
        {
            var requestedUrl = context.HttpContext.Request.Path.Value.Trim('/');
            if (string.IsNullOrWhiteSpace(requestedUrl))
            {
                context.Handler = async ctx => await WriteJsonAsync(ctx, Methods.Keys);
                return Task.CompletedTask;
            }

            requestedUrl = requestedUrl.ToLower();
            //健康状态
            if (requestedUrl == "healthy")
            {
                context.Handler = async ctx => await ctx.Response.WriteAsync("ok");
                return Task.CompletedTask;
            }

            if (!Methods.TryGetValue(requestedUrl, out var method))
            {
                context.Handler = async ctx =>
                    await WriteJsonAsync(ctx, DResult.Error($"{requestedUrl} not found"), (int)HttpStatusCode.NotFound);
                return Task.CompletedTask;
            }
            var instance = CurrentIocManager.Resolve(method.DeclaringType);
            context.Handler = async ctx => await Runner(ctx, instance, method);

            return Task.CompletedTask;
        }

        private static async Task Runner(HttpContext ctx, object instance, MethodBase m)
        {
            string requestBody = null;
            try
            {
                var input = ctx.Request.Body;
                using (var stream = new StreamReader(input))
                {
                    requestBody = stream.ReadToEnd();
                }

                var args = new List<object>();
                JArray list = null;
                if (!string.IsNullOrWhiteSpace(requestBody))
                    list = JsonConvert.DeserializeObject<JArray>(requestBody);
                var i = 0;
                foreach (var parameter in m.GetParameters())
                {
                    if (list != null && list.Count > i)
                    {
                        var parameterType = parameter.ParameterType;
                        if (parameterType.IsSimpleType())
                        {

                            args.Add(list[i].Value<string>()?.CastTo(parameterType));
                        }
                        else
                        {
                            var arg = list[i].ToObject(parameterType);
                            args.Add(arg);
                        }
                    }
                    else
                    {
                        args.Add(parameter.DefaultValue);
                    }
                    i++;
                }

                var result = m.Invoke(instance, args.ToArray());
                await WriteJsonAsync(ctx, result);
            }
            catch (Exception ex)
            {
                var result = ExceptionHandler.Handler(ex, requestBody);
                if (result == null)
                    return;
                await WriteJsonAsync(ctx, result, (int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary> 获取虚拟路径 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}
