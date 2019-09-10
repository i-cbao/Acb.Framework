using System;

namespace Acb.MicroService.PureClient
{
    public class MicroClientException : Exception
    {
        public int Code { get; }

        public MicroClientException(string message, int code) : base(message)
        {
            Code = code;
        }
    }
}
