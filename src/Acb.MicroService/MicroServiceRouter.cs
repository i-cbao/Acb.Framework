using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Reflection;
using Acb.Core.Serialize;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Acb.MicroService
{
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
            var m = instance.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (m == null)
                return Task.CompletedTask;
            context.Handler = async ctx =>
            {
                var input = ctx.Request.Body;
                string html;
                using (var stream = new StreamReader(input))
                {
                    html = stream.ReadToEnd();
                }

                var arg = JsonConvert.DeserializeObject(html, m.GetParameters()[0].ParameterType);

                var result = m.Invoke(instance, new[] { arg });

                var response = ctx.Response;
                response.ContentType = "application/json";
                var bytes = Encoding.ASCII.GetBytes(JsonHelper.ToJson(result));
                await response.Body.WriteAsync(bytes, 0, bytes.Length);
            };
            return Task.CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}
