using Acb.Core.Security;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

            var identity = new ClaimsIdentity();
            if (req.Headers.TryGetValue(AcbClaimTypes.HeaderUserId, out var userId))
                identity.AddClaim(new Claim(AcbClaimTypes.UserId, userId));
            if (req.Headers.TryGetValue(AcbClaimTypes.HeaderTenantId, out var tenantId))
                identity.AddClaim(new Claim(AcbClaimTypes.TenantId, tenantId));
            if (req.Headers.TryGetValue(AcbClaimTypes.HeaderUserName, out var userName))
                identity.AddClaim(new Claim(AcbClaimTypes.UserName, userName));
            if (req.Headers.TryGetValue(AcbClaimTypes.HeaderRole, out var role))
                identity.AddClaim(new Claim(AcbClaimTypes.Role, role));
            HttpContext.User.AddIdentity(identity);
            await _serviceRunner.MicroTask(req, resp, contract, method);
        }
    }
}
