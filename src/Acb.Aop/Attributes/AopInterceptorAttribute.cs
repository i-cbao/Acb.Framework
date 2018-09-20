using System;
using AspectCore.Extensions.Reflection;

namespace Acb.Aop.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AopInterceptorAttribute : Attribute
    {
        public virtual object Invoke(object @object, string method, object[] parameters)
        {
            return @object.GetType().GetMethod(method)?.GetReflector().Invoke(@object, parameters);
        }
    }
}
