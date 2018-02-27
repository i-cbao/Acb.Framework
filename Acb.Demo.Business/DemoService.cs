using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;

namespace Acb.Demo.Business
{
    public class DemoService : IDemoService
    {
        public DemoDto Hello(DemoInputDto dto)
        {
            return new DemoDto
            {
                Demo = dto.Demo,
                Name = dto.Name,
                Time = dto.Time
            };
        }
    }
}
