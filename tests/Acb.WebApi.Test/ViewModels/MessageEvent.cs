using Acb.Core.EventBus;

namespace Acb.WebApi.Test.ViewModels
{
    public class MessageEvent : DEvent
    {
        public string Message { get; set; }
    }
}
