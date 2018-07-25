using System;

namespace Acb.Core.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        protected readonly ISubscriptionManager SubscriptionManager;

        protected AbstractEventBus(ISubscriptionManager manager)
        {
            SubscriptionManager = manager ?? new DefaultSubscriptionManager();
        }
        public abstract void Subscribe<T, TH>(Func<TH> handler) where TH : IIntegrationEventHandler<T>;

        public void Unsubscribe<T, TH>() where TH : IIntegrationEventHandler<T>
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
        }

        public abstract void Publish(IntegrationEvent @event);
    }
}
