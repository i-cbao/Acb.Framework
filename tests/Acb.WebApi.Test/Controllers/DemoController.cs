using Acb.Core;
using Acb.Core.Helper;
using Acb.Demo.Contracts;
using Acb.Demo.Contracts.Dtos;
using Acb.MicroService.Client;
using Acb.Sword.Contracts;
using Acb.Sword.Contracts.Dtos;
using Acb.WebApi.Test.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 实例 </summary>
    [Route("api/[controller]")]
    public class DemoController : DController
    {
        private readonly IDemoService _demoService;

        public DemoController()
        {
            //sendCom
            _demoService = ProxyService.Proxy<IDemoService>();
        }

        /// <summary>
        /// Demo测试接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("hello")]//, AppTicket]
        public DResult<DemoDto> Hello(VDemoInput input)
        {
            var inputDto = Mapper.Map<DemoInputDto>(input);
            var dto = _demoService.Hello(IdentityHelper.Guid32, inputDto);
            return DResult.Succ(dto);
        }

        /// <summary> 店铺列表 </summary>
        /// <returns></returns>
        [HttpGet("list"), AllowAnonymous]
        public DResults<ShopDto> List()
        {
            var shopService = ProxyService.Proxy<IShopService>();

            var dtos = shopService.ShopList(new ShopListInput());
            return DResult.Succ(dtos.List, dtos.Total);
        }

        [HttpGet("token"), AllowAnonymous]
        public DResult<string> Token()
        {
            var ticket = new DemoClientTicket
            {
                Id = 1001L,
                Name = "shay",
                Role = 999
            };
            var token = ticket.Ticket();
            //var cacheKey = $"ticket:{ticket.Id}";
            //AuthorizeCache.Set(cacheKey, ticket.Ticket, TimeSpan.FromMinutes(30));
            return DResult.Succ(token);
        }
    }
}