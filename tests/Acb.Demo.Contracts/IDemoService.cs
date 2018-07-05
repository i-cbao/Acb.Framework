using Acb.Core;
using Acb.Core.Dependency;
using Acb.Demo.Contracts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Demo.Contracts
{
    public interface IDemoService : IDependency, IMicroService
    {
        Task<DemoDto> Hello(string id, DemoInputDto dto);

        IList<string> List(IEnumerable<string> ids);

        Task<Dictionary<string, object>> Dict(string[] ids);

        Task<Dictionary<string, object>> Areas(string parentCode);

        void Load(string id);

        Task LoadAsync();
    }
}
