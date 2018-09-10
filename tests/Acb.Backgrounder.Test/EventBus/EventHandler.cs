using Acb.Core.EventBus;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Demo.Contracts.EventBus;
using Acb.RabbitMq;
using System.Threading.Tasks;

namespace Acb.Backgrounder.Test.EventBus
{
    //[Subscription("icb_framework_simple_queue")]
    //public class MessageHandler : IEventHandler<string>
    //{
    //    private readonly ILogger _logger;

    //    public MessageHandler()
    //    {
    //        _logger = LogManager.Logger<MessageHandler>();
    //    }
    //    public Task Handle(string @event)
    //    {
    //        _logger.Info(@event);
    //        return Task.CompletedTask;
    //    }
    //}

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
            _logger.Info("icb_handler_user_01");
            _logger.Info(JsonHelper.ToJson(@event));
            //throw new Exception("exception Test");
            return Task.CompletedTask;
        }
    }

    [Subscription("icb_handler_user_02")]
    public class AnotherMessageHandler : IEventHandler<UserEvent>
    {
        private readonly ILogger _logger;

        public AnotherMessageHandler()
        {
            _logger = LogManager.Logger<AnotherMessageHandler>();
        }

        public Task Handle(UserEvent @event)
        {
            _logger.Info("icb_handler_user_02");
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
