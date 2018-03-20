using Acb.Core.Dependency;
using Acb.Demo.Contracts.Dtos;

namespace Acb.Demo.Contracts
{
    public interface IDemoService : IDependency
    {
        DemoDto Hello(DemoInputDto dto);
    }
}
