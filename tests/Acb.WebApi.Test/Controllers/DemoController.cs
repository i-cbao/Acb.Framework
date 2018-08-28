using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Core.Logging;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.EventBus;
using Acb.WebApi.Test.Repositories;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例接口 </summary>
    public class DemoController : BaseController
    {
        private readonly IDemoService _demoService;
        private readonly IAccountContract _accountContract;

        public DemoController(IDemoService demoService, IAccountContract accountContract)
        {
            //sendCom
            //_demoService = ProxyService.Proxy<IDemoService>();
            _demoService = demoService;
            _accountContract = accountContract;
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
        public async Task<DResult<TAccount>> Token()
        {
            const string id = "70d8f270f4784d599b9425783bfdea67";
            var t = await _accountContract.QueryById(id);
            var logger = LogManager.Logger<DemoController>();
            logger.Info(t);
            await _accountContract.Update(id, "罗勇", null);
            t = await _accountContract.QueryById(id);
            return DResult.Succ(t);
            //var dto = _connection.Connection("icb_main").QueryFirstOrDefault(
            //    "select * from t_account order by create_time desc");

            //var ticket = new DemoClientTicket
            //{
            //    Id = 1001L,
            //    Name = dto.name,
            //    Role = 999
            //};
            //var token = ticket.Ticket();
            ////var cacheKey = $"ticket:{ticket.Id}";
            ////AuthorizeCache.Set(cacheKey, ticket.Ticket, TimeSpan.FromMinutes(30));
            //return DResult.Succ(JsonHelper.ToJson(ticket));
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