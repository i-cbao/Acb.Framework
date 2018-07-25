using Acb.Core.Dependency;
using System.Threading.Tasks;

namespace Acb.Core.EventBus
{
    public interface IIntegrationEventHandler : ISingleDependency
    {
    }

    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    {
        Task Handle(TIntegrationEvent @event);
    }
}
