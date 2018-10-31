using Acb.Core.Logging;
using AspectCore.DynamicProxy;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acb.Core.Serialize;

namespace Acb.Framework.Tests.Aop
{
    public class LogInterceptor : AbstractInterceptorAttribute
    {
        private readonly ILogger _logger;

        public LogInterceptor()
        {
            _logger = LogManager.Logger<LogInterceptor>();
            _logger.Info("Init LogInterceptor");
        }

        //public override void Before(string method, object[] parameters)
        //{
        //    _watcher.Restart();
        //    _logger.Info("Log Interceptor Before");
        //    base.Before(method, parameters);
        //}

        //public override object After(string method, object result)
        //{
        //    _watcher.Stop();
        //    _logger.Info($"Log Interceptor After,{result},{_watcher.ElapsedMilliseconds}ms");
        //    return base.After(method, result);
        //}

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var watcher = new Stopwatch();
            watcher.Restart();
            try
            {
                _logger.Info("Log Interceptor Before");
                await next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                watcher.Stop();
                object result;
                if (context.IsAsync())
                    result = await context.UnwrapAsyncReturnValue();
                else result = context.ReturnValue;
                _logger.Info($"Log Interceptor After,{JsonHelper.ToJson(result)},{watcher.ElapsedMilliseconds}ms");
            }
        }
    }
}
