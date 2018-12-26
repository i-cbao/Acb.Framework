using System;

namespace Acb.ProxyGenerator
{
    public class ProxyException : Exception
    {
        public ProxyContext Context { get; }

        public ProxyException(ProxyContext context, Exception innerException)
            : base($"Exception has been thrown by the aspect of an invocation. ---> {innerException?.Message}.", innerException)
        {
            Context = context;
        }
    }
}
