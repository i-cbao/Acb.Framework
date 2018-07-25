using Acb.Core.EventBus;

namespace Acb.WebApi.Test.ViewModels
{
    public class MessageEvent : IntegrationEvent
    {
        public string Message { get; set; }
    }
}
