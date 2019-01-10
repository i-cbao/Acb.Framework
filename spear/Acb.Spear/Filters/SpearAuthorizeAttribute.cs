using Acb.Core;
using Acb.Spear.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.Filters
{
    public class SpearAuthorizeAttribute : ActionFilterAttribute
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

            var project = context.HttpContext.GetProject();
            if (project == null)
            {
                Forbidden();
                return;
            }

            if (project.Security == SecurityEnum.None)
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }

            if ((project.Security & SecurityEnum.Get) == 0)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor action &&
                    action.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(ConfigGetAttribute)))
                {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
            }

            var ticket = context.HttpContext.GetTicket();
            if (ticket == null)
            {
                Forbidden();
                return;
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
