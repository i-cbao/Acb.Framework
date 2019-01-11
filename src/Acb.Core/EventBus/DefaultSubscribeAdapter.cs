using Acb.Core.Logging;
using System;
using System.Collections.Generic;

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
