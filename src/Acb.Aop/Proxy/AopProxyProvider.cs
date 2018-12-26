using Acb.Core.Serialize;
using Acb.ProxyGenerator;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acb.Aop.Proxy
{
    [Naming("aop")]
    public class AopProxyProvider
    {
        public object Invoke(MethodInfo method, IDictionary<string, object> parameters, object key = null)
        {
            throw new NotImplementedException();
        }
    }
}
