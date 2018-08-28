using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Demo.Contracts.EventBus;
using Acb.RabbitMq;
using System.Threading.Tasks;

namespace Acb.Backgrounder.Test.EventBus
{
    [Subscription("icb_handler_user_01")]
    public class MessageHandler : IEventHandler<UserEvent>
    {
        private readonly ILogger _logger;

        public MessageHandler()
        {
            _logger = LogManager.Logger<MessageHandler>();
        }

        public Task Handle(UserEvent @event)
        {
            _logger.Info(JsonHelper.ToJson(@event));
            return Task.CompletedTask;
        }
    }

    [Subscription("icb_handler_test")]
    public class MessageHandlerTwo : IEventHandler<TestEvent>
    {
        private readonly ILogger _logger;

        public MessageHandlerTwo()
        {
            _logger = LogManager.Logger<MessageHandler>();
        }

        public Task Handle(TestEvent @event)
        {
            _logger.Info(JsonHelper.ToJson(@event));
            return Task.CompletedTask;
        }
    }
}
