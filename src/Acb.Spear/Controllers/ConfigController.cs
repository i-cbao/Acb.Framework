using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Spear.Domain;
using Acb.Spear.Domain.Dtos;
using Acb.Spear.Domain.Entities;
using Acb.Spear.Domain.Enums;
using Acb.Spear.Filters;
using Acb.Spear.Hubs;
using Acb.Spear.ViewModels;
using Acb.WebApi;
using Acb.WebApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Spear.Controllers
{
    /// <summary> 配置中心 </summary>
    [ConfigAuthorize]
    [ValidateModel]
    [Route("api/config")]
    public class ConfigController : Controller
    {
        private readonly ConfigRepository _repository;
        private readonly IHubContext<ConfigHub> _configHub;
        private readonly ILogger _logger;

        public ConfigController(ConfigRepository repository, IHubContext<ConfigHub> configHub)
        {
            _repository = repository;
            _configHub = configHub;
            _logger = LogManager.Logger<ConfigController>();
        }

        private ConfigTicket _ticket;

        /// <summary> 登录密钥 </summary>
        public ConfigTicket Ticket
        {
            get
            {
                if (_ticket != null)
                    return _ticket;
                var ticket = HttpContext.Request.GetTicket();
                return _ticket = ticket;
            }
        }

        private string _projectCode;

        /// <summary> 项目编码 </summary>
        public string ProjectCode
        {
            get
            {
                if (_projectCode != null)
                    return _projectCode;
                return _projectCode = HttpContext.Request.GetProjectCode();
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
                if (model == "basic")
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(config);
                    var tokens = obj.Children();
                    foreach (var item in tokens)
                    {
                        if (item is JProperty prop)
                        {
                            dict.AddOrUpdate(prop.Name, prop.Value);
                        }
                    }
                }
                else
                {
                    dict.Add(model, JsonConvert.DeserializeObject(config));
                }
            }
            return dict;
        }

        /// <summary> 删除配置 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpDelete("{module}/{env}")]
        public async Task<DResult> RemoveConfig(string module, string env)
        {
            var result = await _repository.DeleteConfig(ProjectCode, module, env);
            return result > 0 ? DResult.Success : DResult.Error("删除失败");
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
            return DResult.Succ(names.OrderBy(t => t), -1);
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

        /// <summary> 还原历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("history/{id}")]
        public async Task<DResult> RecoveryHistory(string id)
        {
            var result = await _repository.RecoveryHistory(id);
            if (result == null)
                return DResult.Error("还原版本失败");
            var config = JsonConvert.DeserializeObject(result.Content);
            await _configHub.Clients.Group($"{ProjectCode}_{result.Name}_{result.Mode}")
                .SendAsync("UPDATE", config);
            return DResult.Success;
        }

        /// <summary> 删除历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("history/{id}")]
        public async Task<DResult> RemoveHistory(string id)
        {
            var dto = await _repository.QueryByIdAsync(id);
            if (dto == null)
                return DResult.Error("版本不存在");
            if (dto.Status != (byte)ConfigStatus.History)
                return DResult.Error("只能删除历史版本");
            var result = await _repository.DeleteAsync(id);
            return result > 0 ? DResult.Success : DResult.Error("删除失败");
        }

        /// <summary> 添加项目 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("project"), AllowAnonymous]
        public async Task<DResult> AddProject([FromBody]VConfigProjectInput input)
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
            if (result > 0)
            {
                var group = $"{ProjectCode}_{module}_{env}";
                _logger.Info($"Hub Group:{group} Update");
                await _configHub.Clients.Group(group).SendAsync("UPDATE", config);
                return DResult.Success;
            }
            return DResult.Error("保存配置失败");
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
                //ExpiredTime = Clock.Now.AddDays(7)
            };
            var ticket = client.Ticket();
            return DResult.Succ(ticket);
        }
    }
}