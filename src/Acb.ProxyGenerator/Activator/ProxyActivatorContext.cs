using System.Reflection;

namespace Acb.ProxyGenerator.Activator
{
    /// <summary> 代理执行器上下文 </summary>
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