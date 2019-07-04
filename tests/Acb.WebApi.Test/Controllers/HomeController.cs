using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.EventBus;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Demo.Contracts.EventBus;
using Acb.Office;
using Acb.RabbitMq.Options;
using Acb.WebApi.Filters;
using Acb.WebApi.Test.Hubs;
using Acb.WebApi.Test.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 主页接口 </summary>
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IRemoteLogger _remoteLogger;
        private readonly IHubContext<MessageHub> _messageHub;

        public HomeController(IHubContext<MessageHub> mhub, IRemoteLogger remoteLogger)
        {
            _messageHub = mhub;
            _logger = LogManager.Logger<HomeController>();
            _remoteLogger = remoteLogger;
            var bus = CurrentIocManager.Resolve<IEventBus>();
        }

        // GET api/values
        [HttpGet]
        public DResults<string> Get()
        {
            return Succ(new[] { "value1", "value2" }, -1);
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
            await ExcelHelper.Export(new DataSet { Tables = { dt } }, "在口袋里的.xls");
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
