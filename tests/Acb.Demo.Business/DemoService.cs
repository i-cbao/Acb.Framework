using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Session;
using Acb.Core.Timing;
using Acb.Demo.Business.Domain;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Demo.Business
{
    public class DemoService : DService, IDemoService
    {
        public AreaRepository AreaRepository { private get; set; }
        public AnotherAreaRepository AnotherAreaRepository { private get; set; }

        private readonly IAcbSession _acbSession;

        public DemoService(IAcbSession acbSession)
        {
            var repo = Resolve<AreaRepository>();
            _acbSession = acbSession;
            Logger.Debug($"{GetType().Name} Create");
        }

        public Task<DateTime> Now()
        {
            Logger.Debug(Clock.Now);
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
            Logger.Info($"load:{id}");
        }

        public Task LoadAsync()
        {
            Logger.Info("loadasync");
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

        public IDictionary<string, string> GetSession()
        {
            var dict = new Dictionary<string, string>
            {
                {"userId", _acbSession.GetUserId<string>()},
                {"tenantId", _acbSession.GetTenantId<string>()},
                {"userName", _acbSession.UserName},
                {"userRole", _acbSession.Role}
            };
            return dict;
        }
    }
}
