using Acb.Core;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例 </summary>
    [Route("api/[controller]")]
    public class DemoController : BaseController
    {
        private readonly IDemoService _demoService;

        public DemoController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        [HttpGet(""), HttpPost("")]//, AppTicket]
        public DResult Hello(VDemoInput input)
        {
            var inputDto = Mapper.Map<DemoInputDto>(input);
            var dto = _demoService.Hello(inputDto);
            var model = Mapper.Map<VDemo>(dto);
            var result = DResult.Succ(new
            {
                demo = model,
                client = Client
            });
            return result;
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
            var cacheKey = $"ticket:{ticket.Id}";
            AuthorizeCache.Set(cacheKey, ticket.Ticket, TimeSpan.FromMinutes(30));
            return DResult.Succ(token);
        }
    }
}