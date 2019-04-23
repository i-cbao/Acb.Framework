using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acb.MicroService.Host.Controller
{
    [Route("micro")]
    public class MicroController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IServiceProvider _provider;

        public MicroController(IServiceProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public async Task Index()
        {
            await MicroServiceRunner.Methods(ControllerContext.HttpContext);
        }

        [HttpPost("{contract}/{method}")]
        public async Task Index(string contract, string method)
        {
            var req = ControllerContext.HttpContext.Request;
            var resp = ControllerContext.HttpContext.Response;
            await MicroServiceRunner.MicroTask(req, resp, contract, method, _provider);
        }
    }
}
