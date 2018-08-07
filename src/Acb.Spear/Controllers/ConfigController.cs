using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.Middleware.JobScheduler.Domain;
using Acb.Middleware.JobScheduler.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Entities;
using Acb.Middleware.JobScheduler.Domain.Enums;
using Acb.Middleware.JobScheduler.Filters;
using Acb.Middleware.JobScheduler.ViewModels;
using Acb.WebApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acb.WebApi;

namespace Acb.Middleware.JobScheduler.Controllers
{
    /// <summary> 配置中心 </summary>
    [ConfigAuthorize]
    [ValidateModel]
    [Route("api/config")]
    public class ConfigController : Controller
    {
        private readonly ConfigRepository _repository;

        public ConfigController(ConfigRepository repository)
        {
            _repository = repository;
        }

        private string ProjectCode
        {
            get
            {
                if (ControllerContext.HttpContext.Request.Headers.TryGetValue("project", out var code))
                    return code.ToString();
                return string.Empty;
            }
        }

        /// <summary> 获取配置 </summary>
        /// <param name="modules">多个以,分割</param>
        /// <param name="env">模式</param>
        /// <returns></returns>
        [HttpGet("{modules}/{env}"), ConfigGet]
        public async Task<Dictionary<string, object>> Index(string modules, string env)
        {
            var list = modules.Split(',');
            var dict = new Dictionary<string, object>();
            foreach (var model in list)
            {
                var config = await _repository.QueryConfig(ProjectCode, model, env);
                if (config == null)
                    continue;
                dict.Add(model, JsonConvert.DeserializeObject(config));
            }
            return dict;
        }

        /// <summary> 获取配置 </summary>
        /// <param name="modules">多个以,分割</param>
        /// <param name="env">模式</param>
        /// <returns></returns>
        [HttpGet("version/{modules}/{env}"), ConfigGet]
        public async Task<Dictionary<string, string>> Versions(string modules, string env)
        {
            var list = modules.Split(',');
            var dict = new Dictionary<string, string>();
            foreach (var model in list)
            {
                var version = await _repository.QueryConfigVersion(ProjectCode, model, env);
                dict.Add(model, version);
            }
            return dict;
        }

        /// <summary> 获取项目所有配置 </summary>
        [HttpGet("list")]
        public async Task<DResults<string>> Configs()
        {
            var names = await _repository.QueryNames(ProjectCode);
            return DResult.Succ(names, -1);
        }

        /// <summary> 获取配置历史版本 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("history/{module}/{env?}")]
        public async Task<DResults<ConfigDto>> History(string module, string env = null, int page = 1, int size = 10)
        {
            var histories = await _repository.QueryHistory(ProjectCode, module, env, page, size);
            var dtos = histories.List.Select(config => new ConfigDto
            {
                Id = config.Id,
                Md5 = config.Md5,
                Timestamp = config.Timestamp,
                Name = config.Name,
                Config = JsonConvert.DeserializeObject(config.Content),
                Desc = config.Desc
            });
            return DResult.Succ(dtos, histories.Total);
        }

        /// <summary> 保存项目 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("project"), AllowAnonymous]
        public async Task<DResult> SaveProject([FromBody]VConfigProjectInput input)
        {
            var model = input.MapTo<TConfigProject>();
            model.Password = model.Password.Md5();
            var result = await _repository.InsertProject(model);
            return result > 0 ? DResult.Success : DResult.Error("保存项目失败");
        }

        /// <summary> 保存项目 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("project")]
        public async Task<DResult> EditProject([FromBody]VConfigProjectEditInput input)
        {
            var model = input.MapTo<TConfigProject>();
            model.Password = model.Password.Md5();
            model.Code = ProjectCode;
            var result = await _repository.UpdateProject(model);
            return result > 0 ? DResult.Success : DResult.Error("保存项目失败");
        }

        /// <summary> 保存配置 </summary>
        /// <param name="module"></param>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpPost("{module}/{env?}")]
        public async Task<DResult> Save(string module, [FromBody]object config, string env = null)
        {
            if (!string.IsNullOrWhiteSpace(env))
            {
                env = env.ToLower();
                var envs = Enum.GetValues(typeof(ConfigEnv)).Cast<ConfigEnv>().Select(t => t.ToString().ToLower());
                if (!envs.Contains(env))
                    return DResult.Error($"不支持的配置模式:{env}");

            }
            var model = new TConfig
            {
                ProjectCode = ProjectCode,
                Name = module,
                Mode = env,
                Status = (byte)ConfigStatus.Normal,
                Content = JsonConvert.SerializeObject(config)
            };
            var result = await _repository.SaveConfig(model);
            return result > 0 ? DResult.Success : DResult.Error("保存配置失败");
        }

        /// <summary> 项目登录 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<DResult<string>> Login([FromBody]VConfigLoginInput input)
        {
            var model = await _repository.ProjectLogin(input.Account, input.Password);
            var client = new ConfigTicket
            {
                Code = model.Code,
                ExpiredTime = Clock.Now.AddDays(7)
            };
            var ticket = client.Ticket();
            return DResult.Succ(ticket);
        }
    }
}