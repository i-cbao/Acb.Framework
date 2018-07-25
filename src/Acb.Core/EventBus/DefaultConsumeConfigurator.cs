using Acb.Core.Dependency;
using Acb.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.Core.EventBus
{
    public class DefaultConsumeConfigurator : IConsumeConfigurator
    {
        private readonly IEventBus _eventBus;

        public DefaultConsumeConfigurator(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public void Configure(List<Type> consumers)
        {
            foreach (var consumer in consumers)
            {
                if (consumer.GetTypeInfo().IsGenericType)
                {
                    continue;
                }
                var consumerType = consumer.GetInterfaces()
                    .Where(
                        d =>
                            d.GetTypeInfo().IsGenericType &&
                            d.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
                    .Select(d => d.GetGenericArguments().Single())
                    .First();
                try
                {
                    //var type = consumer;
                    this.FastInvoke(new[] { consumerType, consumer },
                        x => x.ConsumerTo<object, IIntegrationEventHandler<object>>());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected void ConsumerTo<TEvent, TConsumer>()
            where TConsumer : IIntegrationEventHandler<TEvent>
            where TEvent : class
        {
            _eventBus.Subscribe<TEvent, TConsumer>(CurrentIocManager.Resolve<TConsumer>);
        }
    }
}
