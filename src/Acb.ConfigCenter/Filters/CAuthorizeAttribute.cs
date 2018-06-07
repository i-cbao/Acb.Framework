using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;

namespace Acb.ConfigCenter.Filters
{
    public class CAuthorizeAttribute : ActionFilterAttribute
    {


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor.FilterDescriptors.Any(t => t.Filter.GetType() == typeof(AllowAnonymousFilter)))
            {
                base.OnActionExecuting(context);
                return;
            }

            var config = context.HttpContext.RequestServices.GetService<ConfigManager>();
            var dto = config.GetSecurity();
            if (dto == null || !dto.Enabled)
                return;
            if (dto.Get)
            {
                var action = context.ActionDescriptor as ControllerActionDescriptor;
                if (action?.ControllerName == "Home" && action.ActionName == "Config")
                    return;
            }
            var verify = Helper.VerifyTicket(context.HttpContext.Request, dto);
            if (!verify)
            {
                context.Result =
                    new JsonResult(new { code = 403, message = "forbidden" })
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
