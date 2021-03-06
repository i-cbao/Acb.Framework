﻿using System;
using System.Collections.Generic;

namespace Acb.EventBus
{
    public class DefaultSubscriptionAdapter : ISubscriptionAdapter
    {
        private readonly IConsumeConfigurator _consumeConfigurator;
        private readonly IEnumerable<IEventHandler> _integrationEventHandler;
        public DefaultSubscriptionAdapter(IConsumeConfigurator consumeConfigurator, IEnumerable<IEventHandler> integrationEventHandler)
        {
            _consumeConfigurator = consumeConfigurator;
            _integrationEventHandler = integrationEventHandler;
        }

        public void SubscribeAt()
        {
            _consumeConfigurator.Configure(GetQueueConsumers());
        }

        private List<Type> GetQueueConsumers()
        {
            var result = new List<Type>();
            foreach (var consumer in _integrationEventHandler)
            {
                var type = consumer.GetType();
                result.Add(type);
            }
            return result;
        }
    }
}
