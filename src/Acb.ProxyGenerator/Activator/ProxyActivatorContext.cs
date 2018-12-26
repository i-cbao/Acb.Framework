using System.Reflection;

namespace Acb.ProxyGenerator.Activator
{
    public struct ProxyActivatorContext
    {
        public MethodInfo ServiceMethod { get; }

        public MethodInfo TargetMethod { get; }

        public MethodInfo ProxyMethod { get; }

        public object TargetInstance { get; }

        public object ProxyInstance { get; }

        public object[] Parameters { get; }

        public ProxyActivatorContext(MethodInfo serviceMethod, MethodInfo targetMethod, MethodInfo proxyMethod,
            object targetInstance, object proxyInstance, object[] parameters)
        {
            ServiceMethod = serviceMethod;
            TargetMethod = targetMethod;
            ProxyMethod = proxyMethod;
            TargetInstance = targetInstance;
            ProxyInstance = proxyInstance;
            Parameters = parameters;
        }
    }
}