using System;

namespace Acb.ProxyGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AsyncProxyAttribute : Attribute
    {
    }
}