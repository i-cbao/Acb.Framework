using Acb.Aop;
using Acb.Aop.Attributes;
using Acb.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class AopTest : DTest
    {
        public class CacheInterceptor : AopInterceptorAttribute
        {
            public override object Invoke(object @object, string method, object[] parameters)
            {
                LogManager.Logger<CacheInterceptor>().Info("Cache Interceptor");
                var result = base.Invoke(@object, method, parameters);
                LogManager.Logger<CacheInterceptor>().Info($"Cache result,{result}");
                return result;
            }
        }

        public class LogInterceptor : AopActionAttribute
        {
            public override void Before(string method, object[] parameters)
            {
                LogManager.Logger<CacheInterceptor>().Info("Log Interceptor Before");
                base.Before(method, parameters);
            }

            public override object After(string method, object result)
            {
                LogManager.Logger<CacheInterceptor>().Info($"Log Interceptor After,{result}");
                return base.After(method, result);
            }
        }

        public interface IHelloService
        {
            string Hello(string name);
        }

        public class HelloService : IHelloService
        {
            public string Hello(string name)
            {
                return $"Hello {name}";
            }
        }

        public AopTest()
        {
        }

        [TestMethod]
        public void Test()
        {
            var service = AopProxy.Create<IHelloService, HelloService>(typeof(CacheInterceptor), typeof(LogInterceptor));
            var result = service.Hello("510000");
            Print(result);
        }
    }
}
