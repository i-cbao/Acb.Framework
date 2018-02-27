using Acb.Core;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.WebApi.Filters;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Controllers
{
    
    [Route("api/demo")]
    public class DemoController : DController
    {
        private readonly IDemoService _demoService;

        public DemoController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        [HttpGet(""),HttpPost("")]//, AppTicket]
        public async Task<DResult<VDemo>> Hello([FromBody]VDemoInput input)
        {
            var inputDto = Mapper.Map<DemoInputDto>(input);
            var dto = _demoService.Hello(inputDto);
            var model = Mapper.Map<VDemo>(dto);
            return await Task.FromResult(DResult.Succ(model));
        }
    }
}