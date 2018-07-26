using Acb.Core.Dependency;
using System.Threading.Tasks;

namespace Acb.Core.EventBus
{
    public interface IEventHandler : ISingleDependency
    {
    }

    public interface IEventHandler<in TEvent> : IEventHandler
    {
        Task Handle(TEvent @event);
    }
}
