using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Acb.WebApi.Filters
{
    public class DExceptionFilter : IExceptionFilter
    {
        private const string DefaultErrorMsg = "服务器心情不好， 请稍后重试~";

        public void OnException(ExceptionContext context)
        {
            DResult json;
            var ex = context.Exception.GetBaseException();
            if (ex is BusiException businessEx)
            {
                json = new DResult(false, businessEx.Message, businessEx.Code);
            }
            else
            {
                var logger = LogManager.Logger<DExceptionFilter>();
                logger.Error(ex.Message, ex);
                json = new DResult(false, DefaultErrorMsg, -1);
            }
            const int code = (int)HttpStatusCode.InternalServerError;
            context.Result = new ObjectResult(json)
            {
                StatusCode = code
            };
            context.HttpContext.Response.StatusCode = code;
            context.ExceptionHandled = true;
        }
    }
}
