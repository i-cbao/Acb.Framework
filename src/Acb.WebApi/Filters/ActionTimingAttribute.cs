using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Acb.WebApi.Filters
{
    /// <summary> Action执行监控 </summary>
    public class ActionTimingAttribute : ActionFilterAttribute
    {
        private const string Prefix = "__timer__";
        private const string ThresoldConfig = "actionTimingThreshold";
        private const string ActionKey = "action";
        private const string RenderKey = "render";
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
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            GetTimer(filterContext, ActionKey).Stop();
            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            GetTimer(filterContext, RenderKey).Start();
            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var renderTimer = GetTimer(filterContext, RenderKey);
            renderTimer.Stop();
            var actionTimer = GetTimer(filterContext, ActionKey);
            var elapsedThrold = ThresoldConfig.Config(100);
            if (actionTimer.ElapsedMilliseconds >= elapsedThrold || renderTimer.ElapsedMilliseconds >= elapsedThrold)
            {
                var logger = LogManager.Logger<ActionTimingAttribute>();
                var controller = filterContext.RouteData.Values["controller"];
                var action = filterContext.RouteData.Values["action"];
                var url = Utils.RawUrl(filterContext.HttpContext.Request);
                logger.Warn(string.Format(
                    $"运营监控{controller}_{action},执行:{actionTimer.ElapsedMilliseconds}ms,渲染:{renderTimer.ElapsedMilliseconds}ms,url:{url}"));
            }

            base.OnResultExecuted(filterContext);
        }
    }
}
