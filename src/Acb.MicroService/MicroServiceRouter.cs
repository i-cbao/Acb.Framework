using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Reflection;
using Acb.Core.Serialize;
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
using System.Text;
using System.Threading.Tasks;

namespace Acb.MicroService
{
    /// <summary> 微服务路由 </summary>
    public class MicroServiceRouter : IRouter
    {
        private static List<Type> _services;

        public static List<Type> GetServices()
        {
            if (_services != null)
                return _services;
            _services = CurrentIocManager.Resolve<ITypeFinder>()
                .Find(t => typeof(IMicroService).IsAssignableFrom(t) && t.IsInterface && t != typeof(IMicroService))
                .ToList();
            return _services;
        }

        public Task RouteAsync(RouteContext context)
        {
            var requestedUrl = context.HttpContext.Request.Path.Value.Trim('/');
            var arrs = requestedUrl.Split('/');
            if (arrs == null || arrs.Length != 2)
                return Task.CompletedTask;
            var service = arrs[0];
            var method = arrs[1];
            var type = GetServices().FirstOrDefault(t =>
                string.Equals(t.Name, service, StringComparison.CurrentCultureIgnoreCase));
            if (type == null)
                return Task.CompletedTask;
            var instance = CurrentIocManager.Resolve(type);
            var m = instance.GetType()
                .GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (m == null)
                return Task.CompletedTask;
            context.Handler = async ctx => await Runner(ctx, instance, m);

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
                        var arg = list[i].ToObject(parameterType);
                        args.Add(arg);
                    }
                    else
                    {
                        args.Add(parameter.DefaultValue);
                    }
                    i++;
                }

                var result = m.Invoke(instance, args.ToArray());

                var response = ctx.Response;
                response.ContentType = "application/json";
                var bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(result));
                await response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                var result = ExceptionHandler.Handler(ex, requestBody);
                if (result == null)
                    return;
                const int code = (int)HttpStatusCode.InternalServerError;
                var response = ctx.Response;
                response.ContentType = "application/json";
                response.StatusCode = code;
                var bytes = Encoding.UTF8.GetBytes(JsonHelper.ToJson(result));
                await response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}
