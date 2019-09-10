using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.PureClient.Proxy
{
    public abstract class ProxyAsync
    {

        protected static Type GetReturnType(MethodInfo method)
        {
            var returnType = method.ReturnType;
            if (typeof(Task<>).IsGenericAssignableFrom(returnType))
            {
                returnType = returnType.GenericTypeArguments[0];
            }
            return returnType;
        }

        /// <summary> 调用前 </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task<object> InvokeBeforeAsync(MethodInfo method, object[] args)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary> 调用后 </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual async Task InvokeAfterAsync(MethodInfo method, object[] args, object result)
        {
            await Task.CompletedTask;
        }

        /// <summary> 基础调用 </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract Task<object> BasicInvokeAsync(MethodInfo method, object[] args);

        /// <summary> 切面调用 </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task<object> AopInvokeAsync(MethodInfo method, object[] args)
        {
            var result = await InvokeBeforeAsync(method, args) ?? await BasicInvokeAsync(method, args);
            await InvokeAfterAsync(method, args, result);
            return result;
        }

        public virtual object Invoke(MethodInfo method, object[] args)
        {
            return AopInvokeAsync(method, args).GetAwaiter().GetResult();
        }

        public virtual Task InvokeAsync(MethodInfo method, object[] args)
        {
            return AopInvokeAsync(method, args);
        }

        public virtual async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            var result = await AopInvokeAsync(method, args);
            return result.CastTo<T>();
        }
    }
}
