using Acb.Core;
using Acb.Core.Dependency;
using Acb.Demo.Contracts.Dtos;

namespace Acb.Demo.Contracts
{
    public interface IDemoService : IDependency, IMicroService
    {
        DemoDto Hello(string id, DemoInputDto dto);
    }
}
