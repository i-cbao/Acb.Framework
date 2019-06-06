using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Monitor;
using Acb.Core.Timing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using MonitorManager = Acb.Core.Monitor.MonitorManager;

namespace Acb.WebApi.Filters
{
    /// <summary> Action执行监控 </summary>
    public class ActionTimingFilter : ActionFilterAttribute
    {
        private const string Key = "__timer__action";

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var data = new MonitorData("gateway");
            context.HttpContext.Items.Add(Key, data);
            return base.OnActionExecutionAsync(context, next);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (!context.Items.TryGetValue(Key, out var data) || !(data is MonitorData monitorData))
            {
                base.OnResultExecuted(filterContext);
                return;
            }

            var input = context.Request.Body;
            using (var stream = new StreamReader(input))
            {
                monitorData.Data = stream.ReadToEnd();
            }

            var url = Utils.RawUrl(context.Request);
            var arr = url.Split('?');
            if (arr.Length > 1)
            {
                url = arr[0];
                if (!string.IsNullOrWhiteSpace(monitorData.Data))
                    monitorData.Data += ",";
                monitorData.Data += arr[1];
            }

            monitorData.Url = url;

            if (context.Request.Headers.TryGetValue("referer", out var from))
                monitorData.Referer = from;
            monitorData.CompleteTime = Clock.Now;
            context.RequestServices.Monitor(monitorData);
            base.OnResultExecuted(filterContext);
        }
    }
}
