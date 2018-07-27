using Acb.Core.EventBus;
using Acb.Core.Serialize;
using Acb.Demo.Contracts.EventBus;
using System;
using System.Threading.Tasks;

namespace Acb.Backgrounder.Test.EventBus
{
    [Subscription("icb_handler_user_01")]
    public class MessageHandler : IEventHandler<UserEvent>
    {
        public Task Handle(UserEvent @event)
        {
            Console.WriteLine("one:" + JsonHelper.ToJson(@event));
            return Task.CompletedTask;
        }
    }

    [Subscription("icb_handler_test")]
    public class MessageHandlerTwo : IEventHandler<TestEvent>
    {
        public Task Handle(TestEvent @event)
        {
            Console.WriteLine("two:" + JsonHelper.ToJson(@event));
            return Task.CompletedTask;
        }
    }
}
