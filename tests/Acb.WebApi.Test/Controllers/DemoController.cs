using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.EventBus;
using Acb.WebApi.Test.Connections;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Acb.Core.Helper.Http;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例接口 </summary>
    public class DemoController : BaseController
    {
        private readonly IDemoService _demoService;
        private readonly IConnectionStruct _connection;

        public DemoController(IDemoService demoService, IConnectionStruct conn)
        {
            //sendCom
            //_demoService = ProxyService.Proxy<IDemoService>();
            _demoService = demoService;
            _connection = conn;
        }

        /// <summary> Test </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DResult<DemoDto>> Test([FromBody]VTestInput input)
        {
            input = FromBody<VTestInput>();
            var inputDto = Mapper.Map<DemoInputDto>(input);
            var dto = await _demoService.Hello(IdentityHelper.Guid32, inputDto);
            return DResult.Succ(dto);
        }

        /// <summary>
        /// Demo测试接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("hello")]//, AppTicket]
        public async Task<DResult<DemoDto>> Hello(VDemoInput input)
        {
            var inputDto = Mapper.Map<DemoInputDto>(input);
            var dto = await _demoService.Hello(IdentityHelper.Guid32, inputDto);
            return DResult.Succ(dto);
        }

        [HttpGet("token"), AllowAnonymous]
        public DResult<string> Token()
        {
            var dto = _connection.Connection("icb_main").QueryFirstOrDefault(
                "select * from t_account order by create_time desc");

            var ticket = new DemoClientTicket
            {
                Id = 1001L,
                Name = dto.name,
                Role = 999
            };
            var token = ticket.Ticket();
            //var cacheKey = $"ticket:{ticket.Id}";
            //AuthorizeCache.Set(cacheKey, ticket.Ticket, TimeSpan.FromMinutes(30));
            return DResult.Succ(JsonHelper.ToJson(ticket));
        }

        [HttpGet("kafak"), AllowAnonymous]
        public async Task<string> Kafka(string message)
        {
            var bus = CurrentIocManager.Resolve<IEventBus>();
            await bus.Publish(new TestEvent { Content = message });
            return await Task.FromResult(message);
        }

        [HttpGet("download"), AllowAnonymous]
        public async Task<ActionResult> Download()
        {
            const string url = "http://img.i-cbao.com/finance.pdf";
            var resp = await HttpHelper.Instance.GetAsync(url);
            var stream = await resp.Content.ReadAsStreamAsync();
            return File(stream, "application/pdf", "icb.pdf");
        }
    }
}