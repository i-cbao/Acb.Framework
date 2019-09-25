using System;
using Acb.Core.EventBus;
using Acb.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.RabbitMq.Options;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class RabbitMqTest : DTest
    {
        protected override void MapServices(IServiceCollection services)
        {
            var config = new RabbitMqConfig
            {
                Host = "121.43.162.235",
                Port = 56720,
                VirtualHost = "icb",
                Broker = "icb_event_bus",
                User = "eventbus",
                Password = "80e23de98a09f2ad"
            };
            services.AddRabbitMqEventBus(config);
            base.MapServices(services);
        }

        [TestMethod]
        public async Task PublishTest()
        {
            var ids = new[]
            {
                new[] {"59fbc18331ccc72ca6c308d7402a70bc", "2019-09-24 13:32:12"},
                new[] {"8127064795ffc2a1dd2e08d73f5e79b3", "2019-09-23 13:12:09"},
                new[] {"3dc953e5454acabe332108d73f348c5c", "2019-09-23 08:12:02"},
                new[] {"8d4c2fd0133cc2bd10db08d73e9f4268", "2019-09-22 14:23:23"},
                new[] {"e0d6e0b5bd3bc7ec876b08d73e83709b", "2019-09-22 11:04:14"},
                new[] {"d99f3209b84fc6cf24d408d73e758857", "2019-09-22 09:24:41"},
                new[] {"b098fed4e584c6be539008d73db316f5", "2019-09-21 10:12:48"},
                new[] {"cf5d844f22bec8dc47ab08d73daf741f", "2019-09-21 09:46:47"},
                new[] {"a261b2efeda1c114354408d73d2f6e2d", "2019-09-20 18:30:21"},
                new[] {"763e9cedde4dce8c4d2208d73cec79fa", "2019-09-20 10:31:05"},
                new[] {"428e210d1684c553d3df08d73c7eca00", "2019-09-19 21:25:54"},
                new[] {"8b9e93e0ca28cedbea2608d73c707e6f", "2019-09-19 19:43:35"},
                new[] {"079316f9c9a6c39c58a708d73c6aa058", "2019-09-19 19:01:35"},
                new[] {"715799beae24c213e11a08d73c4a68a4", "2019-09-19 15:10:57"},
                new[] {"dafbdac0118ece2ad42d08d73c2ab782", "2019-09-19 11:24:06"},
                new[] {"1c513822ef64c1dbe41c08d73bc4a13d", "2019-09-18 23:13:20"},
                new[] {"ef25c3bf0341cc6f47bf08d73bc34580", "2019-09-18 23:03:36"},
                new[] {"1376a04929a0ca09d0fe08d73b4e3e14", "2019-09-18 09:05:53"},
                new[] {"42b94314c381c676088008d73ae56f08", "2019-09-17 20:35:38"},
                new[] {"4e068c3d4885c6def43f08d73ac87ebf", "2019-09-17 17:08:29"},
                new[] {"3cb84acda6f2cc0311cb08d73ac7b16f", "2019-09-17 17:02:44"},
                new[] {"7d4af94d2f0acd6efb2108d73aa48274", "2019-09-17 12:50:53"},
                new[] {"9ed1e06ba352ca45b1b208d73a9ea3a5", "2019-09-17 12:08:52"},
                new[] {"6286c954fb96cd7d9c9708d73a8d0436", "2019-09-17 10:02:43"},
                new[] {"d6f398b54a33cd91063f08d7394a1a89", "2019-09-15 19:31:13"},
                new[] {"9899d1bfab96c0bb434b08d7394a0e36", "2019-09-15 19:30:52"},
                new[] {"96f798fe46b3c8de3a6c08d739252646", "2019-09-15 15:06:41"},
                new[] {"021442d754b6c9f7dd9508d738feb33d", "2019-09-15 10:31:27"},
                new[] {"e2af0e72b839c11e522008d7387e7c8e", "2019-09-14 19:13:40"},
                new[] {"6a2e765480aacff5d0bd08d73862a60f", "2019-09-14 15:54:23"},
                new[] {"38669a5b846fce180ecf08d737dff038", "2019-09-14 00:18:44"},
                new[] {"9b2e87afa57ac224e27308d737b04e37", "2019-09-13 18:37:46"},
                new[] {"b94b19d2d5dfc887f1c208d7379e1ac9", "2019-09-13 16:27:28"},
                new[] {"598c91ed4baacd5121f408d736f9f183", "2019-09-12 20:52:22"},
                new[] {"f958ca3c4274c5ccc07e08d736a958dc", "2019-09-12 11:15:26"},
                new[] {"413ca8af3110c05189e308d7369ebb26", "2019-09-12 09:59:26"},
                new[] {"b862ba78d415c3ae287c08d7363c353b", "2019-09-11 22:14:11"},
                new[] {"96c81b92b1edcb54c6a808d7360e53e0", "2019-09-11 16:45:46"}
            };
            var eventBus = Resolve<IEventBus>();
            foreach (var expired in ids)
            {
                var id = expired[0];
                var time = expired[1].CastTo<DateTime>();
                if (time > Clock.Now)
                {
                    await eventBus.Publish("order_expired", new { Id = id }, new RabbitMqPublishOption
                    {
                        Delay = time - Clock.Now
                    });
                }
                else
                {
                    await eventBus.Publish("order_expired", new { Id = id });
                }
            }
        }
    }
}
