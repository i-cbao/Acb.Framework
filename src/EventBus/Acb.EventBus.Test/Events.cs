using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Acb.EventBus.Test
{
    [RouteKey("icb_event_user")]
    public class UserEvent : DEvent
    {
        public string Name { get; set; }
    }

    [Subscription("icb_handler_user_01")]
    public class UserEventHandler : IEventHandler<UserEvent>
    {
        public Task Handle(UserEvent @event)
        {
            return Task.Run(() =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(@event));
                //throw new Exception("test");
            });
        }
    }

    [Subscription("icb_handler_user_02")]
    public class UserEventTwoHandler : IEventHandler<UserEvent>
    {
        public Task Handle(UserEvent @event)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("two:" + JsonConvert.SerializeObject(@event));
                //throw new Exception("test");
            });
        }
    }
}
