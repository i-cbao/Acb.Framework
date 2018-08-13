using Acb.Core.EventBus;
using System;
using System.Threading.Tasks;

namespace Acb.RocketMq
{
    public class EventBusRocketMq : AbstractEventBus, IDisposable
    {
        private readonly IRocketMqConnection _connection;

        public EventBusRocketMq(ISubscriptionManager manager, IRocketMqConnection connection) : base(manager)
        {
            _connection = connection;
        }

        public override Task Subscribe<T, TH>(Func<TH> handler)
        {
            throw new NotImplementedException();
        }

        public override Task Publish(string key, object @event)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
