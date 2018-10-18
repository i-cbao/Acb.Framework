using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Office;
using Acb.WebApi.Test.Hubs;
using Acb.WebApi.Test.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Data;
using System.Threading.Tasks;
using Acb.Core.Dependency;
using Acb.Core.EventBus;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 主页接口 </summary>
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IHubContext<MessageHub> _messageHub;

        public HomeController(IHubContext<MessageHub> mhub)
        {
            _messageHub = mhub;
            _logger = LogManager.Logger<HomeController>();
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
        [HttpGet("{key}")]
        public async Task<DResult<string>> Get(string key)
        {
            Response.Clear();
            var n = key.Replace("-", ":").Config<string>();
            _logger.Info(n);
            _logger.Error(n, new Exception("ex test"));
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
    }
}
