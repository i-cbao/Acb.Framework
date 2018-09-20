using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Helper;
using Acb.Core.Helper.Http;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.Demo.Contracts.EventBus;
using Acb.MicroService.Client;
using Acb.WebApi.Test.Repositories;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            _demoService = ProxyService.Proxy<IDemoService>();
            //_demoService = demoService;
            _accountContract = accountContract;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var time = await _demoService.Now();
            return Content(time.ToString("yyyy-MM-dd HH:mm:ss"));
            //var list = new List<Dictionary<string, object>>();
            //for (int i = 0; i < 3; i++)
            //{
            //    var dict = await _demoService.Areas("510100");
            //    list.Add(dict);
            //}
            //var result = await _demoService.Update();

            //return Json(DResult.Succ(result));
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
        public async Task<IActionResult> Token()
        {
            var ticket =
                "lGn6zHw7xLIgvL1zIUE8W3/xf9LKgLY4TO9HsMfAaBmnZDDmPTIuInBm0W0toN9X5fRcoSm1tBSxPqyQRB45B1rLmoCANocrYoAU/fhk2fGRN2Yv6XG7Pu2EtATJhTOo3pkijgmq1T/0GLIWiLIA1F4+/ydMZv64suytHaw8xrCFzTgQML00GQ9CUS+OBVbnXbUoiiR3dSCgHFvNENR1+QfF9+c1oo/yoVE6Bk6WcnR1MFd0J9Zh08OYHDdcTgiv";
            var client = ticket.Client<DClientTicket<string>>();
            return await Task.FromResult(Json(client));
            //const string id = "70d8f270f4784d599b9425783bfdea67";
            //var t = await _accountContract.QueryById(id);
            //var logger = LogManager.Logger<DemoController>();
            //logger.Info(t);
            //await _accountContract.Update(id, "罗勇", null);
            //t = await _accountContract.QueryById(id);
            //return DResult.Succ(t);
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