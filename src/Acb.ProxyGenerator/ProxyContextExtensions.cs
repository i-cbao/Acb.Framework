using Acb.ProxyGenerator.Attributes;
using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.ProxyGenerator
{
    public static class ProxyContextExtensions
    {
        private static readonly ConcurrentDictionary<MethodInfo, bool> IsAsyncCache = new ConcurrentDictionary<MethodInfo, bool>();

        internal static readonly ConcurrentDictionary<MethodInfo, MethodReflector> ReflectorTable = new ConcurrentDictionary<MethodInfo, MethodReflector>();

        public static async Task AwaitIfAsync(this ProxyContext aspectContext, object returnValue)
        {
            if (returnValue == null)
            {
                return;
            }
            if (returnValue is Task task)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    throw aspectContext.ProxyException(ex);
                }
            }
        }

        public static ProxyException ProxyException(this ProxyContext context, Exception exception)
        {
            if (exception is ProxyException proxyException)
            {
                return proxyException;
            }
            return new ProxyException(context, exception);
        }

        public static bool IsAsync(this ProxyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var isAsyncFromMetaData = IsAsyncCache.GetOrAdd(context.ServiceMethod, IsAsyncFromMetaData);
            if (isAsyncFromMetaData)
            {
                return true;
            }
            if (context.ReturnValue != null)
            {
                return IsAsyncType(context.ReturnValue.GetType().GetTypeInfo());
            }
            return false;
        }

        public static Task<object> UnwrapAsyncReturnValue(this ProxyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (!context.IsAsync())
            {
                throw new ProxyException(context, new InvalidOperationException("This operation only support asynchronous method."));
            }
            var returnValue = context.ReturnValue;
            if (returnValue == null)
            {
                return null;
            }
            var returnTypeInfo = returnValue.GetType().GetTypeInfo();
            return Unwrap(returnValue, returnTypeInfo);
        }

        private static async Task<object> Unwrap(object value, TypeInfo valueTypeInfo)
        {
            object result = null;

            if (valueTypeInfo.IsTaskWithResult())
            {
                // Is there better solution to unwrap ?
                result = (object)(await (dynamic)value);
            }
            else if (valueTypeInfo.IsValueTask())
            {
                // Is there better solution to unwrap ?
                result = (object)(await (dynamic)value);
            }
            else if (value is Task)
            {
                return null;
            }
            else
            {
                result = value;
            }

            if (result == null)
            {
                return null;
            }

            var resultTypeInfo = result.GetType().GetTypeInfo();
            if (IsAsyncType(resultTypeInfo))
            {
                return Unwrap(result, resultTypeInfo);
            }
            return result;
        }

        private static bool IsAsyncFromMetaData(MethodInfo method)
        {
            if (IsAsyncType(method.ReturnType.GetTypeInfo()))
            {
                return true;
            }
            if (method.IsDefined(typeof(AsyncProxyAttribute), true))
            {
                if (method.ReturnType == typeof(object))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAsyncType(TypeInfo typeInfo)
        {
            //return typeInfo.IsTask() || typeInfo.IsTaskWithResult() || typeInfo.IsValueTask();
            if (typeInfo.IsTask())
            {
                return true;
            }
            if (typeInfo.IsTaskWithResult())
            {
                return true;
            }
            if (typeInfo.IsValueTask())
            {
                return true;
            }
            return false;
        }
    }
}