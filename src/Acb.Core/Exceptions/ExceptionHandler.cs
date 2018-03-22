using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Acb.Core.Exceptions
{
    /// <summary> 异常处理类 </summary>
    public static class ExceptionHandler
    {
        private static readonly ILogger Logger = LogManager.Logger(typeof(ExceptionHandler));

        /// <summary> 异常事件 </summary>
        public static event Action<Exception> OnException;

        /// <summary> 业务异常事件 </summary>
        public static event Action<BusiException> OnBusiException;

        private static string ParseParams(IFormCollection nvc)
        {
            return nvc.FromForm().ToUrl();
        }

        private class LogErrorMsg
        {
            public string Message { private get; set; }
            public string Url { private get; set; }
            public string Form { private get; set; }

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(Url))
                    return Message;
                return string.IsNullOrWhiteSpace(Form) ? $"{Message},url:{Url}" : $"{Message},url:{Url},form:{Form}";
            }
        }

        /// <summary> 异常处理 </summary>
        /// <param name="exception"></param>
        public static DResult Handler(Exception exception)
        {
            DResult result = null;
            var ex = exception.GetBaseException();
            switch (ex)
            {
                case BusiException busi:
                    OnBusiException?.Invoke(busi);
                    result = DResult.Error(busi.Message, busi.Code);
                    break;
                case OperationCanceledException _:
                    //操作取消
                    break;
                default:
                    OnException?.Invoke(ex);
                    var msg = new LogErrorMsg
                    {
                        Message = ex.Message
                    };
                    if (AcbHttpContext.Current != null)
                    {
                        msg.Url = AcbHttpContext.RawUrl;
                        if (AcbHttpContext.Form.Any())
                        {
                            msg.Form = ParseParams(AcbHttpContext.Form);
                        }
                    }

                    Logger.Error(msg.ToString(), ex);
                    result = ErrorCodes.SystemError.CodeResult();
                    break;
            }

            return result;
        }
    }
}
