using Acb.Core.Dependency;
using Acb.Core.Logging;
using Acb.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acb.Core.Extensions;
using Acb.Core.Serialize;

namespace Acb.Core.EventBus
{
    public class DefaultConsumeConfigurator : IConsumeConfigurator
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public DefaultConsumeConfigurator(IServiceProvider provider)
        {
            _provider = provider;
            _logger = LogManager.Logger<DefaultConsumeConfigurator>();
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
                            d.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .Select(d => d.GetGenericArguments().Single())
                    .First();
                try
                {
                    this.FastInvoke(new[] { consumerType, consumer },
                        x => x.ConsumerTo<object, IEventHandler<object>>());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        protected void ConsumerTo<TEvent, TConsumer>()
            where TConsumer : IEventHandler<TEvent>
            where TEvent : class
        {
            var consumer = CurrentIocManager.Resolve<TConsumer>();
            var name = consumer.GetType().GetCustomAttribute<NamingAttribute>();
            var eventBus = _provider.GetEventBus(name?.Name);
            eventBus.Subscribe<TEvent, TConsumer>(() => consumer);
        }
    }
}
