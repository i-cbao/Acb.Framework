using Acb.Core;
using Acb.Core.Monitor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.IO;

namespace Acb.WebApi.Filters
{
    /// <summary> Action执行监控 </summary>
    public class ActionTimingFilter : ActionFilterAttribute
    {
        private const string Prefix = "__timer__";
        private const string ActionKey = "action";

        private static Stopwatch GetTimer(ActionContext context, string name)
        {
            var key = Prefix + name;
            if (context.HttpContext.Items.ContainsKey(key))
            {
                return (Stopwatch)context.HttpContext.Items[key];
            }

            var result = new Stopwatch();
            context.HttpContext.Items[key] = result;
            return result;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GetTimer(filterContext, ActionKey).Start();
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var actionTimer = GetTimer(filterContext, ActionKey);
            actionTimer.Stop();
            string data;
            var input = filterContext.HttpContext.Request.Body;
            using (var stream = new StreamReader(input))
            {
                data = stream.ReadToEnd();
            }
            var url = Utils.RawUrl(filterContext.HttpContext.Request);
            filterContext.HttpContext.Request.Headers.TryGetValue("referer", out var from);
            var monitor = MonitorManager.Monitor();
            monitor.Record("gateway", url, from, actionTimer.ElapsedMilliseconds, data, AcbHttpContext.UserAgent,
                AcbHttpContext.ClientIp).Wait();
            base.OnResultExecuted(filterContext);
        }
    }
}
