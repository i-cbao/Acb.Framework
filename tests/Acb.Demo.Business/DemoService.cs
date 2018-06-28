using Acb.Core;
using Acb.Core.Logging;
using Acb.Demo.Business.Domain;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Demo.Business
{
    public class DemoService : IDemoService
    {
        private readonly ILogger _logger = LogManager.Logger<DemoService>();
        public AreaRepository AreaRepository { private get; set; }

        public DemoDto Hello(string id, DemoInputDto dto)
        {
            var t = new DemoDto
            {
                Id = id,
                Demo = dto.Demo,
                Name = dto.Name + ",Success",
                Time = dto.Time
            };
            return t;
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
            var list = AreaRepository.QueryArea("510100");
            return list.ToDictionary(k => k.Id, v => (object)v);
            //return ids.ToDictionary(k => k, v => (object)new { key = v });
        }

        public void Load(string id)
        {
            _logger.Info($"load:{id}");
        }

        public Task LoadAsync()
        {
            _logger.Info("loadasync");
            return Task.FromResult("loadasync");
        }
    }
}
