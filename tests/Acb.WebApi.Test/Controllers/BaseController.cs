using Acb.Core.Cache;
using Acb.Core.Exceptions;
using Acb.WebApi.Test.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 基础身份认证类 </summary>
    //[DAuthorize]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "help")]
    public abstract class BaseController : DAuthorController<DemoClientTicket>
    {
        /// <summary> 当前用户Id </summary>
        public long CurrentId => Client?.Id ?? 0;

        /// <summary> 认证缓存 </summary>
        protected ICache AuthorizeCache => CacheManager.GetCacher("user", CacheLevel.Both, 2);

        /// <summary> 身份验证 </summary>
        /// <param name="context"></param>
        public override void AuthorizeValidate(HttpContext context)
        {
            base.AuthorizeValidate(context);
            if (Client == null)
                throw ErrorCodes.InvalidTicket.CodeException();
            var cacheKey = $"ticket:{Client.Id}";
            var cacheTicket = AuthorizeCache.Get<string>(cacheKey);
            if (!string.Equals(Client.Ticket, cacheTicket, StringComparison.CurrentCultureIgnoreCase))
                throw ErrorCodes.InvalidTicket.CodeException();
            AuthorizeCache.ExpireEntryIn(cacheKey, TimeSpan.FromMinutes(30));
        }
    }
}
