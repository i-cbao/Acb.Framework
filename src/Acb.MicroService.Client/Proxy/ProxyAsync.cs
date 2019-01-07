using Acb.Core.Cache;
using Acb.Core.Extensions;
using Acb.Core.Serialize;
using Acb.Core.Timing;
using Acb.Redis;
using AspectCore.Extensions.Reflection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Client.Proxy
{
    public abstract class ProxyAsync
    {
        private readonly IDatabase _redis = RedisManager.Instance.GetDatabase();
        private bool _cacheSet = true;

        protected static InterceptCacheAttribute GetInterceptCache(MethodInfo method, object[] args)
        {
            var interceptCache = method.GetReflector().GetCustomAttribute<InterceptCacheAttribute>();
            if (interceptCache == null || !string.IsNullOrWhiteSpace(interceptCache.Key))
                return interceptCache;
            interceptCache.Key = "micro:intercept:cache:";
            if (method.DeclaringType != null)
            {
                interceptCache.Key += $"{method.DeclaringType.FullName?.ToLower()}:";
            }

            interceptCache.Key += method.Name.ToLower();
            if (args != null && args.Length > 0)
                interceptCache.Key += $":{JsonHelper.ToJson(args).Md5().ToLower()}";
            return interceptCache;
        }

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
            var interceptCache = GetInterceptCache(method, args);
            if (interceptCache == null || interceptCache.Method != CacheMethod.Get)
                return null;

            var redisValue = await _redis.StringGetAsync(interceptCache.Key);
            if (!redisValue.HasValue)
                return null;
            _cacheSet = false;
            var returnType = GetReturnType(method);
            return returnType.IsSimpleType()
                ? redisValue.ToString()
                : JsonConvert.DeserializeObject(redisValue, returnType);
        }

        /// <summary> 调用后 </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual async Task InvokeAfterAsync(MethodInfo method, object[] args, object result)
        {
            var interceptCache = GetInterceptCache(method, args);
            if (interceptCache == null)
                return;
            switch (interceptCache.Method)
            {
                case CacheMethod.Put:
                case CacheMethod.Get:
                    if (_cacheSet)
                    {
                        TimeSpan expired;
                        if (interceptCache.Time > 0)
                            expired = TimeSpan.FromSeconds(interceptCache.Time);
                        else if (interceptCache.Date > 0)
                            expired = Clock.Now.AddDays(interceptCache.Date) - Clock.Now;
                        await _redis.SetAsync(interceptCache.Key, result, expired);
                    }

                    break;
                case CacheMethod.Remove when interceptCache.RemoveKeys != null && interceptCache.RemoveKeys.Length > 0:
                    var keys = interceptCache.RemoveKeys.Select(t => args != null ? string.Format(t, args) : t).ToArray();
                    foreach (var key in keys)
                    {
                        await _redis.KeyDeleteAsync(key);
                    }
                    break;
                default:
                    break;
            }
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
            return AopInvokeAsync(method, args).SyncRun();
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
