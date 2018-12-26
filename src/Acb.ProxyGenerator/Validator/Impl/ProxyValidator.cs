using AspectCore.Extensions.Reflection;
using System.Linq;
using System.Reflection;

namespace Acb.ProxyGenerator.Validator.Impl
{
    public delegate bool ProxyValidationDelegate(ProxyValidationContext context);

    public sealed class ProxyValidator : IProxyValidator
    {
        private readonly ProxyValidationDelegate _proxyValidationDelegate;

        public ProxyValidator(ProxyValidationDelegate proxyValidationDelegate)
        {
            _proxyValidationDelegate = proxyValidationDelegate;
        }

        public bool Validate(MethodInfo method, bool isStrictValidation)
        {
            if (method == null)
            {
                return false;
            }

            var context = new ProxyValidationContext { Method = method, StrictValidation = isStrictValidation };
            if (_proxyValidationDelegate(context))
            {
                return true;
            }

            var declaringTypeInfo = method.DeclaringType.GetTypeInfo();
            if (!declaringTypeInfo.IsClass)
            {
                return false;
            }

            foreach (var interfaceTypeInfo in declaringTypeInfo.GetInterfaces().Select(x => x.GetTypeInfo()))
            {
                var interfaceMethod = interfaceTypeInfo.GetMethodBySignature(new MethodSignature(method));
                if (interfaceMethod != null)
                {
                    if (Validate(interfaceMethod, isStrictValidation))
                    {
                        return true;
                    }
                }
            }

            var baseType = declaringTypeInfo.BaseType;
            if (baseType == typeof(object) || baseType == null)
            {
                return false;
            }

            var baseMethod = baseType.GetTypeInfo().GetMethodBySignature(new MethodSignature(method));
            return baseMethod != null && Validate(baseMethod, isStrictValidation);
        }
    }
}