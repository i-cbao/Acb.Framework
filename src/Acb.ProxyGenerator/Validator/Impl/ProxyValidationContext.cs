using System;
using System.Reflection;
using Acb.ProxyGenerator.Activator;
using Acb.ProxyGenerator.Activator.Impl;

namespace Acb.ProxyGenerator.Validator.Impl
{
    public struct ProxyValidationContext : IEquatable<ProxyValidationContext>
    {
        public MethodInfo Method { get; set; }

        public bool StrictValidation { get; set; }

        public bool Equals(ProxyValidationContext other)
        {
            return Method == other.Method && StrictValidation == other.StrictValidation;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is ProxyActivatorContext other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            var hash_1 = this.Method?.GetHashCode() ?? 0;
            var hash_2 = StrictValidation.GetHashCode();
            return (hash_1 << 5) + hash_1 ^ hash_2;
        }
    }
}