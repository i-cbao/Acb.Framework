using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Acb.ConfigCenter.Controllers
{
    [Route("")]
    public class HomeController : BaseController
    {
        private readonly ConfigManager _config;

        public HomeController(ConfigManager config)
        {
            _config = config;
        }

        /// <summary> 获取配置 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpGet("{module}/{env}")]
        public ActionResult Config(string module, string env)
        {
            var config = _config.Get(module, env);
            return Json(config);
        }

        /// <summary> 获取配置列表 </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public List<string> List()
        {
            return _config.List();
        }

        /// <summary> 添加配置 </summary>
        /// <param name="file"></param>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpPost("{file}/{env?}")]
        public ActionResult Add(string file, [FromBody]object config, string env = null)
        {
            if (!string.IsNullOrWhiteSpace(env))
                file = $"{file}-{env}";
            var result = _config.Create(file, config);
            return Json(new { ok = result });
        }

        /// <summary> 编辑配置 </summary>
        /// <param name="file"></param>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpPut("{file}/{env?}")]
        public ActionResult Edit(string file, [FromBody]object config, string env = null)
        {
            if (!string.IsNullOrWhiteSpace(env))
                file = $"{file}-{env}";
            var result = _config.Set(file, config);
            return Json(new { ok = result });
        }

        /// <summary> 删除配置 </summary>
        /// <param name="file"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpDelete("{file}/{env?}")]
        public ActionResult Delete(string file, string env = null)
        {
            _config.Remove(file);
            return Json(new { ok = true });
        }
    }
}