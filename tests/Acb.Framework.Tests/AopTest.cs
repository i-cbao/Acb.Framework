using Acb.Core.Helper;
using Acb.Framework.Tests.Aop;
using Acb.Framework.Tests.Service;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class AopTest : DTest
    {
        public AopTest()
        {
        }

        protected override void MapServices(ContainerBuilder builder)
        {
            //builder.RegisterDynamicProxy(config =>
            //{
            //    config.Interceptors.AddTyped<LogInterceptor>(m => m.DeclaringType.Name.EndsWith("Service"));
            //});
            builder.Register(c => new CacheInterceptor());
            builder.RegisterType<HelloService>().As<IHelloService>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheInterceptor));
            base.MapServices(builder);
        }

        [TestMethod]
        public async Task Test()
        {
            var proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            proxyGeneratorBuilder.Configure(config =>
                {
                    config.Interceptors.AddDelegate((ctx, next) => next(ctx), Predicates.ForService("IHelloService"));
                });
            var proxyGenerator = proxyGeneratorBuilder.Build();
            var proxy = proxyGenerator.CreateInterfaceProxy<IHelloService>();

            var t = proxy.Hello("shay");
            Print(t);

            //var service = Resolve<IHelloService>();
            //var result = await service.Hello($"{RandomHelper.Random().Next(50000)}");
            //Print(result);
            //var demoService = Resolve<IDemoService>();
            //var now = await demoService.Now();
            //Print(now);
            //var list = demoService.List(new[] { IdentityHelper.Guid16 });
            //Print(list);
        }
    }
}
