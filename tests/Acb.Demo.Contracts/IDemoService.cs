using System;
using Acb.Core;
using Acb.Core.Cache;
using Acb.Core.Dependency;
using Acb.Demo.Contracts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Demo.Contracts
{
    public interface IDemoService : IDependency, IMicroService
    {
        [InterceptCache(Time = 100)]
        Task<DateTime> Now();

        Task<DemoDto> Hello(string id, DemoInputDto dto);

        IList<string> List(IEnumerable<string> ids);

        Task<Dictionary<string, object>> Dict(string[] ids);

        Task<Dictionary<string, object>> Areas(string parentCode);

        void Load(string id);

        Task LoadAsync();

        Task<int> Update();

        string[] GetSession();
    }
}
