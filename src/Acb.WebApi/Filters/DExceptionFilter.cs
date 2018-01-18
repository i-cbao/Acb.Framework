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
                json = DResult.Error(businessEx.Message, businessEx.Code);
            }
            else
            {
                var logger = LogManager.Logger<DExceptionFilter>();
                logger.Error(ex.Message, ex);
                json = DResult.Error(DefaultErrorMsg);
            }
            const int code = (int)HttpStatusCode.OK;
            context.Result = new ObjectResult(json)
            {
                StatusCode = code
            };
            context.HttpContext.Response.StatusCode = code;
            context.ExceptionHandled = true;
        }
    }
}
