using Acb.Core;
using Acb.Middleware.DatabaseManager.Domain;
using Acb.Middleware.DatabaseManager.Domain.Models;
using Acb.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.Middleware.DatabaseManager.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : DController
    {

        public HomeController()
        {
        }

        [HttpGet("tables/{name?}")]
        public async Task<DResults<Table>> GetTablesAsync(string name = null)
        {
            var service = name.GetService();
            var tables = await service.GetTablesAsync();
            return Succ(tables, -1);
        }

        [HttpGet("/tables/{name?}")]
        public async Task<IActionResult> Tables(string name = null)
        {
            var service = name.GetService();
            var tables = await service.GetTablesAsync();
            ViewBag.DbName = service.DbName;
            ViewBag.Provider = service.Provider;
            return View(tables);
        }
    }
}
