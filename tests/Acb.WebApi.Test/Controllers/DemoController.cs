using Acb.Core;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例 </summary>
    [Route("api/[controller]")]
    public class DemoController : DController
    {
        private readonly IDemoService _demoService;

        public DemoController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        [HttpPost("hello")]//, AppTicket]
        public DemoDto Hello([FromBody]VDemoInput input)
        {
            var inputDto = Mapper.Map<DemoInputDto>(input);
            return _demoService.Hello(inputDto);
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