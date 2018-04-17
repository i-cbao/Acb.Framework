using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Monitor;
using Acb.Core.Serialize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Acb.MicroService
{
    /// <summary> 微服务路由 </summary>
    internal class MicroServiceRunner
    {
        private static async Task RunMethod(MethodBase m, HttpRequest request, HttpResponse response)
        {
            string requestBody = null;
            var watcher = new Stopwatch();
            watcher.Start();
            try
            {
                var input = request.Body;
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
                var instance = CurrentIocManager.Resolve(m.DeclaringType);
                var result = m.Invoke(instance, args.ToArray());
                await WriteJsonAsync(response, result);
            }
            catch (Exception ex)
            {
                var result = ExceptionHandler.Handler(ex, requestBody);
                if (result == null)
                    return;
                await WriteJsonAsync(response, result, (int)HttpStatusCode.InternalServerError);
            }
            finally
            {
                watcher.Stop();
                var monitor = MonitorManager.Monitor();
                var url = Utils.RawUrl(request);
                request.Headers.TryGetValue("referer", out var from);
                await monitor.Record("micro_service", url, from, watcher.ElapsedMilliseconds, requestBody,
                    AcbHttpContext.UserAgent, AcbHttpContext.ClientIp);
            }
        }

        private static async Task WriteJsonAsync(HttpResponse response, object data, int code = (int)HttpStatusCode.OK)
        {
            response.StatusCode = code;
            response.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(data));
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        public static async Task Methods(HttpContext ctx)
        {
            await WriteJsonAsync(ctx.Response, MicroServiceRegister.Methods.Keys);
        }

        public static Task MicroTask(HttpRequest req, HttpResponse resp, string contract, string method)
        {
            var path = $"{contract}/{method}";
            return MicroTask(req, resp, path);
        }

        public static Task MicroTask(HttpRequest req, HttpResponse resp, string path)
        {
            var key = path.ToLower();
            if (!MicroServiceRegister.Methods.TryGetValue(key, out var m))
            {
                return WriteJsonAsync(resp, DResult.Error($"{path} not found"), (int)HttpStatusCode.NotFound);
            }
            return RunMethod(m, req, resp);
        }


        /// <summary> 路由处理 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task RouteAsync(RouteContext context)
        {
            var requestedUrl = context.HttpContext.Request.Path.Value.Trim('/');
            if (string.IsNullOrWhiteSpace(requestedUrl))
            {
                context.Handler = async ctx => await WriteJsonAsync(ctx.Response, MicroServiceRegister.Methods.Keys);
                return Task.CompletedTask;
            }

            requestedUrl = requestedUrl.ToLower();
            //健康状态
            if (requestedUrl == "healthy")
            {
                context.Handler = async ctx => await ctx.Response.WriteAsync("ok");
                return Task.CompletedTask;
            }

            if (!MicroServiceRegister.Methods.TryGetValue(requestedUrl, out var method))
            {
                context.Handler = async ctx =>
                    await WriteJsonAsync(ctx.Response, DResult.Error($"{requestedUrl} not found"), (int)HttpStatusCode.NotFound);
                return Task.CompletedTask;
            }
            var instance = CurrentIocManager.Resolve(method.DeclaringType);
            context.Handler = async ctx => await Runner(ctx, instance, method);

            return Task.CompletedTask;
        }

        private static async Task Runner(HttpContext ctx, object instance, MethodBase m)
        {
            string requestBody = null;
            var watcher = new Stopwatch();
            watcher.Start();
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
                await WriteJsonAsync(ctx.Response, result);
            }
            catch (Exception ex)
            {
                var result = ExceptionHandler.Handler(ex, requestBody);
                if (result == null)
                    return;
                await WriteJsonAsync(ctx.Response, result, (int)HttpStatusCode.InternalServerError);
            }
            finally
            {
                watcher.Stop();
                var monitor = MonitorManager.Monitor();
                var url = Utils.RawUrl(ctx.Request);
                ctx.Request.Headers.TryGetValue("referer", out var from);
                monitor.Record("micro_service", url, from, watcher.ElapsedMilliseconds, requestBody,
                    AcbHttpContext.UserAgent, AcbHttpContext.ClientIp);
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
