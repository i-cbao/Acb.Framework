using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;

namespace Acb.WebApi.Filters
{
    /// <summary> 默认的异常处理 </summary>
    public class DExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger = LogManager.Logger<DExceptionFilter>();

        private static string ParseParams(IFormCollection nvc)
        {
            if (nvc == null || !nvc.Any())
                return string.Empty;
            var list = nvc.Keys.Select(key => $"{key}={nvc[key].ToString().UrlDecode()}").ToList();

            return string.Join("&", list);
        }

        public class LogErrorMsg
        {
            public string Message { get; set; }
            public string Url { get; set; }
            public string Form { get; set; }

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(Url))
                    return Message;
                return string.IsNullOrWhiteSpace(Form) ? $"{Message},url:{Url}" : $"{Message},url:{Url},form:{Form}";
            }
        }
        /// <inheritdoc />
        /// <summary> 异常处理 </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            DResult json = null;
            var ex = context.Exception.GetBaseException();
            switch (ex)
            {
                case BusiException busi:
                    json = DResult.Error(busi.Message, busi.Code);
                    break;
                case OperationCanceledException _:
                    //操作取消
                    break;
                default:
                    var msg = new LogErrorMsg
                    {
                        Message = ex.Message
                    };
                    if (AcbHttpContext.Current != null)
                    {
                        msg.Url = $"{AcbHttpContext.RawUrl}";
                        if (AcbHttpContext.Form.Any())
                        {
                            msg.Form = $"{ParseParams(AcbHttpContext.Form)}";
                        }
                    }

                    _logger.Error(msg.ToString(), ex);
                    json = ErrorCodes.SystemError.CodeResult();
                    break;
            }
            if (json == null) return;
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
