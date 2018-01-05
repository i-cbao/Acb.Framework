namespace Acb.Core.Exceptions
{
    /// <summary> 业务执行中，业务逻辑不满足返回异常 </summary>
    public class BusiException : DException
    {
        /// <summary> 构造函数 </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public BusiException(string message, int code = ErrorCode.SystemError)
            : base(message, code)
        {
        }
    }
}
