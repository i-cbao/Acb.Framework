using System.Threading.Tasks;
using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.MicroService.Client;
using Acb.WebApi.Test.Connections;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例接口 </summary>
    [Route("api/[controller]")]
    public class DemoController : DController
    {
        private readonly IDemoService _demoService;
        private readonly IConnectionStruct _connection;

        public DemoController(IDemoService demoService, IConnectionStruct conn)
        {
            //sendCom
            _demoService = ProxyService.Proxy<IDemoService>();
            _connection = conn;
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
    }
}