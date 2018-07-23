using Acb.Core;
using Microsoft.AspNetCore.Mvc;

namespace Acb.WebApi.Test.Areas.Vehicle.Controllers
{
    [Area("vehicle"), Route("[area]/[controller]")]
    [ApiExplorerSettings(GroupName = "vehicle")]
    public class HomeController : Controller
    {
        /// <summary> vehicle test </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public DResult Test()
        {
            return DResult.Success;
        }
    }
}