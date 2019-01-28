using Acb.Core;
using Acb.Spear.Contracts.Enums;
using Acb.Spear.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Spear.Filters
{
    /// <summary>
    /// 两种认证方式
    /// 1.项目编码 (参数)
    /// 2.Authorization (Header)
    /// </summary>
    public class SpearAuthorizeAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowAnonymousFilter)))
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }

            //禁止访问
            void Forbidden()
            {
                context.Result =
                    new JsonResult(DResult.Error("Forbidden", 401))
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
            }

            var httpContext = context.HttpContext;
            //方式一 参数/header
            var project = httpContext.GetProjectByCode();
            if (project != null)
            {
                if (project.Security == SecurityEnum.None)
                {
                    httpContext.SetProject(project);
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
                //获取时不需要认证
                if ((project.Security & SecurityEnum.Get) == 0)
                {
                    if (context.ActionDescriptor is ControllerActionDescriptor action &&
                        action.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowGetAttribute)))
                    {
                        httpContext.SetProject(project);
                        await base.OnActionExecutionAsync(context, next);
                        return;
                    }
                }
            }

            //方式二 Header
            project = httpContext.GetProjectByToken();
            if (project == null)
            {
                Forbidden();
                return;
            }
            httpContext.SetProject(project);
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
