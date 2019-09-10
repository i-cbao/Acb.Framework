using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Acb.Market.Contracts;
using Acb.Market.Contracts.Dtos;
using Acb.Shield.Contracts;

namespace Acb.PureClient.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly IVCodeContract _codeContract;
        private readonly IFchContract _fchContract;

        public ValuesController(IVCodeContract codeContract, IFchContract fchContract)
        {
            _codeContract = codeContract;
            _fchContract = fchContract;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            //var dto = _codeContract.Generate();
            //return dto;
            var dto = _fchContract.GetThirdByMobileAsync("18270551037").Result;
            return Json(dto);
            //var dto = _fchContract.GetThirdOrderAsync("2001628").Result;
            //return Json(dto);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
