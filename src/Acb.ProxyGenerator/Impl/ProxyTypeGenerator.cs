using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acb.ProxyGenerator.Utils;
using Acb.ProxyGenerator.Validator;
using AspectCore.Extensions.Reflection;

namespace Acb.ProxyGenerator.Impl
{
    public sealed class ProxyTypeGenerator : IProxyTypeGenerator
    {
        private readonly IProxyValidator _validator;
        private readonly ProxyGeneratorUtils _proxyGeneratorUtils;

        public ProxyTypeGenerator(IProxyValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder == null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }
            _validator = validatorBuilder.Build();
            _proxyGeneratorUtils = new ProxyGeneratorUtils();
        }

        public Type CreateClassProxyType(Type serviceType, Type implementationType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (!serviceType.GetTypeInfo().IsClass)
            {
                throw new ArgumentException($"Type '{serviceType}' should be class.", nameof(serviceType));
            }
            return _proxyGeneratorUtils.CreateClassProxy(serviceType, implementationType, GetInterfaces(implementationType).ToArray(), _validator);
        }

        public Type CreateInterfaceProxyType(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (!serviceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException($"Type '{serviceType}' should be interface.", nameof(serviceType));
            }
            return _proxyGeneratorUtils.CreateInterfaceProxy(serviceType, GetInterfaces(serviceType, serviceType).ToArray(), _validator);
        }

        public Type CreateInterfaceProxyType(Type serviceType, Type implementationType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (!serviceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException($"Type '{serviceType}' should be interface.", nameof(serviceType));
            }
            return _proxyGeneratorUtils.CreateInterfaceProxy(serviceType, implementationType, GetInterfaces(implementationType, serviceType).ToArray(), _validator);
        }

        private static IEnumerable<Type> GetInterfaces(Type type, params Type[] exceptInterfaces)
        {
            var hashSet = new HashSet<Type>(exceptInterfaces);
            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces().Distinct())
            {
                if (!interfaceType.GetTypeInfo().IsVisible())
                {
                    continue;
                }
                if (!hashSet.Contains(interfaceType))
                {      
                    if (interfaceType.GetTypeInfo().ContainsGenericParameters && type.GetTypeInfo().ContainsGenericParameters)
                    {
                        if (!hashSet.Contains(interfaceType.GetGenericTypeDefinition()))
                            yield return interfaceType;
                    }
                    else
                    {
                        yield return interfaceType;
                    }
                }
            }
        }

    }
}