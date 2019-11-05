using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Session;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.EventBus;
using Acb.MicroService.Client;
using Acb.Office;
using Acb.RabbitMq.Options;
using Acb.WebApi.Filters;
using Acb.WebApi.Test.Hubs;
using Acb.WebApi.Test.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Acb.Core.Domain.Dtos;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 主页接口 </summary>
    [Route("api/home")]
    public partial class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IRemoteLogger _remoteLogger;
        private readonly IHubContext<MessageHub> _messageHub;
        private readonly IAcbSession _session;

        private readonly IDemoService _demoService;

        public HomeController(IHubContext<MessageHub> mhub, IRemoteLogger remoteLogger, IAcbSession session, IDemoService demoService)
        {
            _messageHub = mhub;
            _logger = LogManager.Logger<HomeController>();
            _remoteLogger = remoteLogger;
            _session = session;
            _demoService = ProxyService.Proxy<IDemoService>(); //demoService;
            var bus = CurrentIocManager.Resolve<IEventBus>();
        }

        [HttpPost("login"), AllowAnonymous]
        public DResult<string> Login([FromBody]VLoginInput input)
        {
            var client = new DClientTicket<Guid>
            {
                Id = IdentityHelper.NewSequentialGuid(),
                Name = input.Account,
                Role = "admin"
            };
            return Succ(client.Ticket());
        }

        // GET api/values
        [HttpGet(), AllowAnonymous]
        public DResult<IDictionary<string, string>> Get(int code = 0, string tenant = null)
        {
            //var claims = new List<Claim>
            //{
            //    new Claim(AcbClaimTypes.UserId, "shay"),
            //    new Claim(AcbClaimTypes.TenantId, "10001")
            //};
            //HttpContext.User.AddIdentity(new ClaimsIdentity(claims));
            //var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            //principal?.AddIdentity(new ClaimsIdentity(claims));
            if (code == 0)
                return Succ(_demoService.GetSession());
            using (_session.Use(new SessionDto
            {
                UserId = "test",
                TenantId = "10002"
            }))
            {
                return Succ(_demoService.GetSession());
            }

            //return Succ(new[] { "value1", "value2" }, -1);
        }

        [HttpGet("test")]
        public DResult<string> Test(VHomeInput input)
        {
            return DResult.Succ(input.Code);
        }

        // GET api/values/5
        [HttpGet("{key}"), RecordFilter("demo_key")]
        public async Task<DResult<string>> Get(string key)
        {
            Response.Clear();
            var n = key.Replace("-", ":").Config<string>();
            _logger.Info(n);
            var obj = new
            {
                msg = n,
                url = "http://www.baidu.com",
                form = "a=1",
                token = "acb 123456"
            };
            var ex = new Exception("ex test");
            _remoteLogger.Logger(obj, LogLevel.Info, ex, logger: GetType().FullName);
            _logger.Error(obj, ex);
            return await Task.FromResult(Succ($"hello {n}"));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        /// <summary> 发送消息 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("send")]
        public async Task<DResult> Send([FromBody]VHomeInput input)
        {
            await _messageHub.Clients.All.SendAsync("send", input.Code);
            return Success;
        }

        /// <summary> 导出 </summary>
        /// <returns></returns>
        [HttpGet("export")]
        public async Task Export()
        {
            var dt = new DataTable("sheet");
            dt.Columns.Add("姓名", typeof(string));
            dt.Rows.Add("shay");
            await HttpContext.Export(new DataSet { Tables = { dt } }, "在口袋里的.xls");
        }

        [HttpPost("event/send")]
        public async Task<DResult> EventHandler(string message)
        {
            var provider = HttpContext.RequestServices;
            var bus = provider.GetService<IEventBus>();
            await bus.Publish(new UserEvent { Name = message }, new RabbitMqPublishOption
            {
                Delay = TimeSpan.FromSeconds(2)
            });

            //bus.Publish("icb_framework_simple_queue", cmd, 2 * 1000);
            _logger.Info($"Send Message:{message}");
            var sbus = provider.GetEventBus("spartner");
            await sbus.Publish(new TestEvent { Content = message }, new RabbitMqPublishOption
            {
                Delay = TimeSpan.FromSeconds(10)
            });
            return Success;
        }
    }
}
