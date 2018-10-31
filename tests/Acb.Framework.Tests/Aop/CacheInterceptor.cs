using System.Reflection;
using System.Threading.Tasks;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Castle.DynamicProxy;

namespace Acb.Framework.Tests.Aop
{
    public class CacheInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public CacheInterceptor()
        {
            _logger = LogManager.Logger<CacheInterceptor>();
        }

        public void Intercept(IInvocation invocation)
        {
            _logger.Info("Cache Interceptor");
            invocation.Proceed();
            var result = invocation.ReturnValue.TaskResult().SyncRun();
            _logger.Info($"Cache result,{result}");
        }
    }
}
