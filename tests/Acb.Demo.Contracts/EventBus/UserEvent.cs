using Acb.Core.EventBus;

namespace Acb.Demo.Contracts.EventBus
{
    [RouteKey("icb_event_user")]
    public class UserEvent : DEvent
    {
        public string Name { get; set; }
    }
}
