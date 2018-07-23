using Acb.Core;
using Microsoft.AspNetCore.Mvc;

namespace Acb.WebApi.Test.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {

        /// <summary> 测试接口 </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public DResult Test()
        {
            return DResult.Success;
        }
    }
}