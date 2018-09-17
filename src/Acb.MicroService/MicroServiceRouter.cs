using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Acb.Core.Dependency;

namespace Acb.MicroService
{
    /// <summary> 微服务路由 </summary>
    public class MicroServiceRouter : IRouter
    {
        /// <summary> 路由处理 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task RouteAsync(RouteContext context)
        {
            var requestedUrl = context.HttpContext.Request.Path.Value.Trim('/');
            if (string.IsNullOrWhiteSpace(requestedUrl))
            {
                context.Handler = async ctx => await MicroServiceRunner.Methods(ctx);
                return Task.CompletedTask;
            }

            requestedUrl = requestedUrl.ToLower();
            //健康状态
            if (requestedUrl == "healthy")
            {
                context.Handler = async ctx => await ctx.Response.WriteAsync("ok");
                return Task.CompletedTask;
            }

            context.Handler = async ctx => await MicroServiceRunner.MicroTask(ctx.Request, ctx.Response, requestedUrl,
                CurrentIocManager.Resolve<IServiceProvider>());

            return Task.CompletedTask;
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
