using Acb.ProxyGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ProxyGeneratorTest : DTest
    {
        public interface ITestService
        {
            string Hello(string name);
        }

        public class TestService : ITestService
        {
            public string Hello(string name)
            {
                return $"Hello {name}";
            }
        }

        protected override void MapServices(IServiceCollection services)
        {
            services.AddProxy();
            base.MapServices(services);
        }

        [TestMethod]
        public void Test()
        {
            var factory = Resolve<IProxyGenerator>();
            var service = factory.CreateInterfaceProxy<ITestService>(new TestService());
            var word = service.Hello("shay");
            Print(word);
        }
    }
}
