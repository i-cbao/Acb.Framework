using Acb.Core;
using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Core.Modules;

namespace Acb.RabbitMq
{
    [DependsOn(typeof(CoreModule))]
    public class RabbitMqModule : DModule
    {
        public override void Initialize()
        {
            IocManager.Resolve<ISubscriptionAdapter>().SubscribeAt();
            LogManager.Logger<RabbitMqModule>().Info("开始订阅...");
        }
    }
}
