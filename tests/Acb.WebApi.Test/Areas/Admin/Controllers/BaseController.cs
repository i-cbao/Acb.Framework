using Microsoft.AspNetCore.Mvc;

namespace Acb.WebApi.Test.Areas.Admin.Controllers
{
    [Area("admin"), Route("[area]/[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    public class BaseController : DController
    {
    }
}
