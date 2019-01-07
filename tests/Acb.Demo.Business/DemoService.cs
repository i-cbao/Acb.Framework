using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Demo.Business.Domain;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DService = Acb.Core.Domain.DService;

namespace Acb.Demo.Business
{
    public class DemoService : DService, IDemoService
    {
        private readonly ILogger _logger = LogManager.Logger<DemoService>();
        public AreaRepository AreaRepository { private get; set; }
        public AnotherAreaRepository AnotherAreaRepository { private get; set; }

        public DemoService()
        {
            _logger.Debug($"{GetType().Name} Create");
        }

        public Task<DateTime> Now()
        {
            _logger.Debug(Clock.Now);
            return Task.FromResult(Clock.Now);
        }

        public async Task<DemoDto> Hello(string id, DemoInputDto dto)
        {
            var t = new DemoDto
            {
                Id = id,
                Demo = dto.Demo,
                Name = dto.Name + ",Success",
                Time = Clock.Now
            };
            return await Task.FromResult(t);
        }

        public IList<string> List(IEnumerable<string> ids)
        {
            var agent = AcbHttpContext.UserAgent;
            var list = (ids?.ToArray() ?? new string[] { }).ToList();
            list.Add(agent);
            return list;
        }

        public async Task<Dictionary<string, object>> Dict(string[] ids)
        {
            //throw new BusiException("dist error");
            return await Task.FromResult(ids.ToDictionary(k => k, v => (object)v));
        }

        public async Task<Dictionary<string, object>> Areas(string parentCode)
        {
            var list = await AreaRepository.QueryAreaAsync(parentCode);
            return list.ToDictionary(k => k.Id, v => (object)v);
        }

        public void Load(string id)
        {
            _logger.Info($"load:{id}");
        }

        public Task LoadAsync()
        {
            _logger.Info("loadasync");
            return Task.CompletedTask;
        }

        public async Task<int> Update()
        {
            return await AreaRepository.UnitOfWork.Trans(async () =>
            {
                var count = await AreaRepository.UpdateName();
                count += await AnotherAreaRepository.UpdateParent();
                return count;
            });
        }
    }
}
