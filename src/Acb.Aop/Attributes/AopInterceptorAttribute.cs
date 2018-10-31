using AspectCore.Extensions.Reflection;
using System;

namespace Acb.Aop.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class AopInterceptorAttribute : Attribute
    {
        public virtual object Invoke(object @object, string method, object[] parameters)
        {
            return @object.GetType().GetMethod(method)?.GetReflector().Invoke(@object, parameters);
        }
    }
}
