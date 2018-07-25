using Acb.Core.EventBus;

namespace Acb.Demo.Contracts.EventBus
{
    public class TestEvent : IntegrationEvent
    {
        public string Content { get; set; }
    }
}
