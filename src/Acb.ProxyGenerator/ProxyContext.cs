using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.ProxyGenerator
{
    public abstract class ProxyContext
    {
        public abstract IDictionary<string, object> AdditionalData { get; }

        public abstract object ReturnValue { get; set; }

        public abstract IServiceProvider ServiceProvider { get; }

        public abstract MethodInfo ServiceMethod { get; }

        public abstract object Implementation { get; }

        public abstract MethodInfo ImplementationMethod { get; }

        public abstract object[] Parameters { get; }

        public abstract MethodInfo ProxyMethod { get; }

        public abstract object Proxy { get; }

        public abstract Task Break();

        public abstract Task Invoke(ProxyDelegate next);

        public abstract Task Complete();
    }
}