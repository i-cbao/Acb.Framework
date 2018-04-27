using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using System.Collections.Generic;
using System.Linq;
using Acb.Core;

namespace Acb.Demo.Business
{
    public class DemoService : IDemoService
    {
        public DemoDto Hello(string id, DemoInputDto dto)
        {
            return new DemoDto
            {
                Id = id,
                Demo = dto.Demo,
                Name = dto.Name + ",Success",
                Time = dto.Time
            };
        }

        public IList<string> List(IEnumerable<string> ids)
        {
            var agent = AcbHttpContext.UserAgent;
            var list = (ids?.ToArray() ?? new string[] { }).ToList();
            list.Add(agent);
            return list;
        }

        public Dictionary<string, object> Dict(string[] ids)
        {
            return ids.ToDictionary(k => k, v => (object)new { key = v });
        }
    }
}
