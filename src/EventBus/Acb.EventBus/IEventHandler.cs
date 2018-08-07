using System.Threading.Tasks;

namespace Acb.EventBus
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<in TEvent> : IEventHandler
    {
        Task Handle(TEvent @event);
    }
}
