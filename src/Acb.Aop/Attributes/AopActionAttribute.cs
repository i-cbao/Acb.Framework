using System;

namespace Acb.Aop.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AopActionAttribute : Attribute
    {
        public virtual void Before(string method, object[] parameters) { }

        public virtual object After(string method, object result) { return result; }
    }
}
