using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.MicroService.Host.Controller
{
    /// <summary> 微服务控制器 </summary>
    [Route("micro")]
    public class MicroController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly MicroServiceRunner _serviceRunner;

        public MicroController(MicroServiceRunner serviceRunner)
        {
            _serviceRunner = serviceRunner;
        }

        /// <summary> 所有服务 </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task Index()
        {
            await _serviceRunner.Methods(ControllerContext.HttpContext);
        }

        /// <summary> 微服务调用入口 </summary>
        /// <param name="contract"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        [HttpPost("{contract}/{method}")]
        public async Task Index(string contract, string method)
        {
            var req = ControllerContext.HttpContext.Request;
            var resp = ControllerContext.HttpContext.Response;
            await _serviceRunner.MicroTask(req, resp, contract, method);
        }
    }
}
