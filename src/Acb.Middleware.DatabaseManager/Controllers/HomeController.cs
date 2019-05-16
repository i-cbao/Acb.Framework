using Acb.Core;
using Acb.Core.Logging;
using Acb.Middleware.DatabaseManager.Domain;
using Acb.Middleware.DatabaseManager.Domain.Models;
using Acb.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            try
            {
                var service = name.GetService();
                var tables = await service.GetTablesAsync();
                ViewBag.DbName = service.DbName;
                ViewBag.Provider = service.Provider;
                ViewBag.Name = name;
                return View(tables);
            }
            catch (Exception ex)
            {
                ViewBag.DbName = ex.Message;
                ViewBag.Name = name;
                LogManager.Logger<HomeController>().Error(ex.Message, ex);
                return View(new List<Table>());
            }
        }
    }
}
