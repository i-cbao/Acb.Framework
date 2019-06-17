﻿using Acb.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.EventBus
{
    public class DefaultSubscribeAdapter : ISubscribeAdapter
    {
        private readonly IConsumeConfigurator _consumeConfigurator;
        private readonly IEnumerable<IEventHandler> _integrationEventHandler;
        private readonly ILogger _logger;
        public DefaultSubscribeAdapter(IConsumeConfigurator consumeConfigurator, IEnumerable<IEventHandler> integrationEventHandler)
        {
            _consumeConfigurator = consumeConfigurator;
            _integrationEventHandler = integrationEventHandler;
            _logger = LogManager.Logger<DefaultSubscribeAdapter>();
        }

        public void SubscribeAt()
        {
            _logger.Info("开启订阅...");
            _consumeConfigurator.Configure(GetQueueConsumers());
        }

        private List<Type> GetQueueConsumers()
        {
            return _integrationEventHandler.Select(consumer => consumer.GetType()).ToList();
        }
    }
}
