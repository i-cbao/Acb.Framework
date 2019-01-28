using Acb.AutoMapper;
using Acb.Core;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using Acb.Spear.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseDto = Acb.Spear.Contracts.Dtos.Database.DatabaseDto;

namespace Acb.Spear.Controllers
{
    /// <summary> 数据库项目接口 </summary>
    [Route("api/database")]
    public class DatabaseController : DController
    {
        private readonly IDatabaseContract _contract;

        public DatabaseController(IDatabaseContract contract)
        {
            _contract = contract;
        }

        /// <summary> 数据库连接列表 </summary>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<DResults<VDatabase>> ListAsync(ProviderType? type = null, int page = 1, int size = 10)
        {
            var dtos = await _contract.PagedListAsync(Ticket.Id, type, page, size);
            var models = dtos.MapPagedList<VDatabase, DatabaseDto>();
            return Succ(models.List, models.Total);
        }

        /// <summary> 添加数据库连接 </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<DResult> AddAsync([FromBody]VDatabaseInput input)
        {
            var result = await _contract.AddAsync(Ticket.Id, input.Name, input.Provider, input.ConnectionString);
            return FromResult(result, "添加数据库连接失败");
        }

        /// <summary> 编辑数据库连接 </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<DResult> EditAsync(Guid id, [FromBody]VDatabaseInput input)
        {
            var result = await _contract.SetAsync(id, input.Name, input.Provider, input.ConnectionString);
            return FromResult(result, "更新数据库连接失败");
        }

        /// <summary> 删除数据库连接 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<DResult> RemoveAsync(Guid id)
        {
            var result = await _contract.RemoveAsync(id);
            return FromResult(result, "删除失败");
        }

        /// <summary> 数据库文档 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/tables/{id:guid}"), AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Tables(Guid id)
        {
            try
            {
                var dto = await _contract.GetAsync(id);
                ViewBag.DbName = dto.DbName;
                ViewBag.Provider = dto.Provider;
                ViewBag.Name = dto.Name;
                return View(dto.Tables);
            }
            catch (Exception ex)
            {
                ViewBag.DbName = ex.Message;
                ViewBag.Name = "";
                return View(new List<TableDto>());
            }
        }
    }
}
