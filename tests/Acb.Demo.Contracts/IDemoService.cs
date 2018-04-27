using Acb.Core;
using Acb.Core.Dependency;
using Acb.Demo.Contracts.Dtos;
using System.Collections.Generic;

namespace Acb.Demo.Contracts
{
    public interface IDemoService : IDependency, IMicroService
    {
        DemoDto Hello(string id, DemoInputDto dto);

        IList<string> List(IEnumerable<string> ids);

        Dictionary<string, object> Dict(string[] ids);
    }
}
