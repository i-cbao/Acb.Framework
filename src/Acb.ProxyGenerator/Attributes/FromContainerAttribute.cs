using System;

namespace Acb.ProxyGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class FromContainerAttribute : Attribute
    {
        public FromContainerAttribute()
        {
        }
    }
}
