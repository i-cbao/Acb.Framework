namespace Acb.Core.Exceptions
{
    public class DException : System.Exception
    {
        public int Code { get; }

        public DException(string message, int code = ErrorCode.SystemError)
            : base(message)
        {
            Code = code;
        }
    }
}
