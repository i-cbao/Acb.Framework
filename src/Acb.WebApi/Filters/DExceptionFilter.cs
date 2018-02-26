using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Acb.WebApi.Filters
{
    /// <summary> 默认的异常处理 </summary>
    public class DExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger = LogManager.Logger<DExceptionFilter>();
        /// <summary> 异常处理 </summary>
        /// <param name="context"></param>
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
                _logger.Error(ex.Message, ex);
                json = ErrorCodes.SystemError.CodeResult<ErrorCodes>();
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
