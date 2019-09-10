using System;
using Acb.Core.EventBus;
using Acb.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Acb.Core.Timing;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class RabbitMqTest : DTest
    {
        protected override void MapServices(IServiceCollection services)
        {
            services.AddRabbitMqEventBus(new RabbitMqConfig
            {
                Host = "121.43.162.235",
                Port = 56720,
                VirtualHost = "icb",
                Broker = "icb_event_bus",
                User = "eventbus",
                Password = "80e23de98a09f2ad"
            });
            base.MapServices(services);
        }

        [TestMethod]
        public async Task PublishTest()
        {
            var eventBus = Resolve<IEventBus>();
            await eventBus.Publish("order_expired", new
            {
                Id = "cf16424019f3c7d428c808d735bcaf84"
            }, DateTime.Parse("2019-09-11 07:01:21") - Clock.Now);
        }
    }
}
