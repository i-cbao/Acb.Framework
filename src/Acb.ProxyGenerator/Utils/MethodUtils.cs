using Acb.ProxyGenerator.Activator;
using AspectCore.Extensions.Reflection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Acb.ProxyGenerator.Utils
{
    internal static class MethodUtils
    {
        internal static readonly MethodInfo MProxyActivator = GetMethod<IProxyActivatorFactory>(nameof(IProxyActivatorFactory.Create));

        internal static readonly MethodInfo MProxyInvoke = GetMethod<IProxyActivator>(nameof(IProxyActivator.Invoke));

        internal static readonly MethodInfo MProxyInvokeTask = GetMethod<IProxyActivator>(nameof(IProxyActivator.InvokeTask));

        internal static readonly MethodInfo MProxyInvokeValueTask = GetMethod<IProxyActivator>(nameof(IProxyActivator.InvokeValueTask));

        internal static readonly ConstructorInfo CtorActivatorContext = typeof(ProxyActivatorContext).GetTypeInfo().DeclaredConstructors.First();

        internal static readonly ConstructorInfo CtorObject = typeof(object).GetTypeInfo().DeclaredConstructors.Single();

        internal static readonly MethodInfo GetParameters = typeof(ProxyActivatorContext).GetTypeInfo().GetMethod("get_Parameters");

        internal static readonly MethodInfo GetMethodReflector = GetMethod<Func<MethodInfo, MethodReflector>>(m => m.GetReflector());

        internal static readonly MethodInfo ReflectorInvoke = GetMethod<Func<MethodReflector, object, object[], object>>((r, i, a) => r.Invoke(i, a));

        private static MethodInfo GetMethod<T>(Expression<T> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new InvalidCastException("Cannot be converted to MethodCallExpression");
            }
            return methodCallExpression.Method;
        }

        private static MethodInfo GetMethod<T>(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return typeof(T).GetTypeInfo().GetMethod(name);
        }
    }
}
