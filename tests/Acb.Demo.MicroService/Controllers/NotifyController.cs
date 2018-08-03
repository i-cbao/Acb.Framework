using Microsoft.AspNetCore.Mvc;

namespace Acb.Demo.MicroService.Controllers
{
    [Route("notify")]
    public class NotifyController : Controller
    {
        [HttpGet, HttpPost]
        public string Index()
        {
            return "notify";
            //var resp = AcbHttpContext.Current.Response;
            //resp.ContentType = "text/html;charset=utf-8";
            //await Task.Run(async () => { await resp.WriteAsync("SUCCESS"); });
        }
    }
}
