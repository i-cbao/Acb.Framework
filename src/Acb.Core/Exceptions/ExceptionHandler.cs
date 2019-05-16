using Acb.Core.Logging;
using System;
using System.IO;

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

        private class LogErrorMsg
        {
            public string Message { get; set; }
            public string Url { get; set; }
            public string Form { get; set; }
            public string Token { get; set; }
            public string ClientIp { get; set; }
            public string UserAgent { get; set; }
        }

        /// <summary> 异常处理 </summary>
        /// <param name="exception"></param>
        /// <param name="requestBody"></param>
        public static DResult Handler(Exception exception, string requestBody = null)
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
                        msg.ClientIp = AcbHttpContext.ClientIp;
                        msg.UserAgent = AcbHttpContext.UserAgent;
                        if (string.IsNullOrWhiteSpace(requestBody))
                        {
                            var input = AcbHttpContext.Body;
                            if (input.CanSeek)
                                input.Seek(0, SeekOrigin.Begin);
                            if (input.CanRead)
                            {
                                using (var stream = new StreamReader(input))
                                {
                                    msg.Form = stream.ReadToEnd();
                                }
                            }
                        }
                        else
                        {
                            msg.Form = requestBody;
                        }

                        if (AcbHttpContext.Current.Request.Headers.TryGetValue("Authorization", out var authorize) &&
                            !string.IsNullOrWhiteSpace(authorize))
                            msg.Token = authorize;
                    }

                    Logger.Error(msg, ex);
                    result = ErrorCodes.SystemError.CodeResult();
                    break;
            }

            return result;
        }
    }
}
