using Acb.AutoMapper;
using Acb.Core;
using Acb.Spear.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Acb.Spear.Business.Domain;
using Acb.Spear.Business.Domain.Entities;

namespace Acb.Spear.Controllers
{
    [Route("api/project")]
    public class ProjectController : DController
    {
        private readonly ProjectRepository _repository;

        public ProjectController(ProjectRepository repository)
        {
            _repository = repository;
        }

        /// <summary> 添加项目 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("project"), AllowAnonymous]
        public async Task<DResult> AddProject([FromBody]VConfigProjectInput input)
        {
            var model = input.MapTo<TProject>();
            var result = await _repository.InsertAsync(model);
            return result > 0 ? DResult.Success : DResult.Error("保存项目失败");
        }

        /// <summary> 保存项目 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("project")]
        public async Task<DResult> EditProject([FromBody]VConfigProjectEditInput input)
        {
            var model = input.MapTo<TProject>();
            model.Code = ProjectCode;
            var result = await _repository.UpdateAsync(model);
            return result > 0 ? DResult.Success : DResult.Error("保存项目失败");
        }
    }
}
