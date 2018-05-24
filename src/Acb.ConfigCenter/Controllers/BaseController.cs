using Acb.ConfigCenter.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Acb.ConfigCenter.Controllers
{
    [CAuthorize]
    [Produces("application/json")]
    public class BaseController : Controller
    {
    }
}