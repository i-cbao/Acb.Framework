using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using System.Collections.Generic;
using System.Linq;

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

        public string[] List(List<string> ids)
        {
            return ids?.ToArray() ?? new string[] { };
        }
    }
}
