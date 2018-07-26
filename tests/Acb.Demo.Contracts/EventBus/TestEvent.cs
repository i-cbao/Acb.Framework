using Acb.Core.EventBus;
using Acb.Core.Serialize;

namespace Acb.Demo.Contracts.EventBus
{
    [RouteKey("icb_event_test")]
    public class TestEvent : DEvent
    {
        public string Content { get; set; }
    }
}
