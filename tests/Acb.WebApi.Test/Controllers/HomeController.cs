using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : DController
    {
        private readonly ILogger _logger = LogManager.Logger<HomeController>();
        // GET api/values
        [HttpGet]
        public DResults<string> Get()
        {
            return Succ(new[] { "value1", "value2" }, -1);
        }

        // GET api/values/5
        [HttpGet("{key}")]
        public async Task<DResult<string>> Get(string key)
        {
            var n = key.Config<string>();
            _logger.Info(n);
            _logger.Error(n);
            return await Task.FromResult(Succ($"hello {n}"));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
