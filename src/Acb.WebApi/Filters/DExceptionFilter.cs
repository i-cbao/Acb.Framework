using Acb.Core;
using Acb.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace Acb.WebApi.Filters
{
    /// <inheritdoc />
    /// <summary> 默认的异常处理 </summary>
    public class DExceptionFilter : IExceptionFilter
    {
        /// <summary> 业务消息过滤 </summary>
        public static Action<DResult> ResultFilter;
        /// <inheritdoc />
        /// <summary> 异常处理 </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var json = ExceptionHandler.Handler(context.Exception);
            if (json == null)
                return;
            ResultFilter?.Invoke(json);
            const int code = (int)HttpStatusCode.OK;
            context.Result = new JsonResult(json)
            {
                StatusCode = code
            };
            context.HttpContext.Response.StatusCode = code;
            context.ExceptionHandled = true;
        }
    }
}
