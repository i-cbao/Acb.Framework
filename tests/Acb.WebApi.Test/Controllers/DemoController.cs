using Acb.Core;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.MicroService.Client;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例 </summary>
    [Route("api/[controller]")]
    public class DemoController : DController
    {
        private readonly IDemoService _demoService;

        public DemoController()
        {
            _demoService = ProxyService.Proxy<IDemoService>();
        }

        /// <summary>
        /// Demo测试接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("hello")]//, AppTicket]
        public DResult<DemoDto> Hello(VDemoInput input)
        {
            var inputDto = Mapper.Map<DemoInputDto>(input);
            var dto = _demoService.Hello(inputDto);
            return DResult.Succ(dto);
        }

        [HttpGet("token"), AllowAnonymous]
        public DResult<string> Token()
        {
            var ticket = new DemoClientTicket
            {
                Id = 1001L,
                Name = "shay",
                Role = 999
            };
            var token = ticket.Ticket();
            //var cacheKey = $"ticket:{ticket.Id}";
            //AuthorizeCache.Set(cacheKey, ticket.Ticket, TimeSpan.FromMinutes(30));
            return DResult.Succ(token);
        }
    }
}