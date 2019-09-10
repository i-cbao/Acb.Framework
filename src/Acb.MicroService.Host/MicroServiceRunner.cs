using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Message;
using Acb.Core.Monitor;
using Acb.Core.Security;
using Acb.Core.Timing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Host
{
    /// <summary> 微服务路由 </summary>
    public class MicroServiceRunner
    {
        private readonly MicroServiceRegister _serviceRegister;
        private readonly IMessageCodec _messageCodec;
        private readonly IServiceProvider _serviceProvider;

        public MicroServiceRunner(MicroServiceRegister serviceRegister, IMessageCodec messageCodec, IServiceProvider serviceProvider)
        {
            _serviceRegister = serviceRegister;
            _messageCodec = messageCodec;
            _serviceProvider = serviceProvider;
        }

        private async Task RunMethod(MethodBase m, HttpRequest request, HttpResponse response)
        {
            var monitorData = new MonitorData(MonitorModules.MicroService)
            {
                Method = request.Method
            };
            string requestBody = null;
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

                var instance = _serviceProvider.GetService(m.DeclaringType); //CurrentIocManager.Resolve(m.DeclaringType);
                var result = m.Invoke(instance, args.ToArray());
                monitorData.Result = result.GetStringResult();
                await WriteJsonAsync(response, result);
            }
            catch (Exception ex)
            {
                monitorData.Code = 500;
                monitorData.Result = ex.Message;
                var result = ExceptionHandler.Handler(ex, requestBody);
                if (result == null)
                    return;
                await WriteJsonAsync(response, result, (int)HttpStatusCode.InternalServerError);
            }
            finally
            {
                monitorData.Url = Utils.RawUrl(request);
                monitorData.Data = requestBody;
                monitorData.CompleteTime = Clock.Now;
                if (request.Headers.TryGetValue(AcbClaimTypes.HeaderReferer, out var from))
                    monitorData.Referer = from;
                _serviceProvider.Monitor(monitorData);
            }
        }

        private async Task WriteJsonAsync(HttpResponse response, object data, int code = (int)HttpStatusCode.OK)
        {
            response.StatusCode = code;
            response.ContentType = "application/json";
            response.Headers.Add("Content-Encoding", "gzip");
            if (data is Task task)
            {
                await task;
                var taskType = task.GetType().GetTypeInfo();
                if (taskType.IsGenericType)
                {
                    var prop = taskType.GetProperty("Result");
                    if (prop != null)
                        data = prop.GetValue(task);
                }
                else
                {
                    return;
                }
            }

            var bytes = _messageCodec.Encode(data, true);
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        public async Task Methods(HttpContext ctx)
        {
            var methods = _serviceRegister.Methods.Select(t => new
            {
                url = t.Key,
                param = t.Value.GetParameters().ToDictionary(k => k.Name, v => v.ParameterType.Name)
            }).OrderBy(t => t.url).ToDictionary(t => t.url, v => v.param);
            await WriteJsonAsync(ctx.Response, methods);
        }

        public Task MicroTask(HttpRequest req, HttpResponse resp, string contract, string method)
        {
            var path = $"{contract}/{method}";
            return MicroTask(req, resp, path);
        }

        public Task MicroTask(HttpRequest req, HttpResponse resp, string path)
        {
            var key = path.ToLower();
            var register = CurrentIocManager.Resolve<MicroServiceRegister>();
            if (!register.Methods.TryGetValue(key, out var m))
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
                context.Handler = async ctx => await WriteJsonAsync(ctx.Response, _serviceRegister.Methods.Keys);
                return Task.CompletedTask;
            }

            requestedUrl = requestedUrl.ToLower();
            //健康状态
            if (requestedUrl == "healthy")
            {
                context.Handler = async ctx => await ctx.Response.WriteAsync("ok");
                return Task.CompletedTask;
            }

            if (!_serviceRegister.Methods.TryGetValue(requestedUrl, out var method))
            {
                context.Handler = async ctx =>
                    await WriteJsonAsync(ctx.Response, DResult.Error($"{requestedUrl} not found"), (int)HttpStatusCode.NotFound);
                return Task.CompletedTask;
            }
            var instance = CurrentIocManager.Resolve(method.DeclaringType);
            context.Handler = async ctx => await Runner(ctx, instance, method);

            return Task.CompletedTask;
        }

        private async Task Runner(HttpContext ctx, object instance, MethodBase m)
        {
            string requestBody = null;
            var monitorData = new MonitorData(MonitorModules.MicroService)
            {
                Method = ctx.Request.Method
            };
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
                monitorData.Result = result.GetStringResult();
                await WriteJsonAsync(ctx.Response, result);
            }
            catch (Exception ex)
            {
                monitorData.Code = 500;
                monitorData.Result = ex.Message;
                var result = ExceptionHandler.Handler(ex, requestBody);
                if (result == null)
                    return;
                await WriteJsonAsync(ctx.Response, result, (int)HttpStatusCode.InternalServerError);
            }
            finally
            {
                monitorData.Url = Utils.RawUrl(ctx.Request);
                monitorData.Data = requestBody;
                monitorData.CompleteTime = Clock.Now;
                if (ctx.Request.Headers.TryGetValue(AcbClaimTypes.HeaderReferer, out var from))
                    monitorData.Referer = from;
                _serviceProvider.Monitor(monitorData);
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
