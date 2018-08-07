using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Timing;
using Acb.Middleware.JobScheduler.Domain;
using Acb.Middleware.JobScheduler.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Middleware.JobScheduler.Filters
{
    /// <summary> 配置获取权限 </summary>
    public class ConfigGetAttribute : ActionFilterAttribute { }

    public class ConfigAuthorizeAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowAnonymousFilter)))
            {
                await base.OnActionExecutionAsync(context, next);
            }

            void Forbidden()
            {
                context.Result =
                    new JsonResult(DResult.Error("Forbidden"))
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
            }

            context.HttpContext.Request.Headers.TryGetValue("project", out var code);
            if (string.IsNullOrWhiteSpace(code))
            {
                Forbidden();
                return;
            }

            var project = await CurrentIocManager.Resolve<ConfigRepository>().QueryProject(code);
            if (project == null)
            {
                Forbidden();
                return;
            }

            if (project.Security == (byte)ConfigSecurity.None)
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }

            if ((project.Security & (byte)ConfigSecurity.Get) == 0)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor action &&
                    action.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(ConfigGetAttribute)))
                {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
            }

            var ticket = context.HttpContext.Request.GetTicket();
            if (ticket == null || ticket.Code != code || (ticket.ExpiredTime.HasValue && ticket.ExpiredTime.Value < Clock.Now))
            {
                Forbidden();
                return;
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
