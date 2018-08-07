using Acb.WebApi;
using Microsoft.AspNetCore.Http;

namespace Acb.Middleware.JobScheduler.Domain
{
    /// <summary> 项目凭证 </summary>
    public class ConfigTicket : ClientTicket
    {
        /// <summary> 项目编码 </summary>
        public string Code { get; set; }
    }
    public static class ConfigTicketHelper
    {
        /// <summary> 验证令牌 </summary>
        /// <param name="request"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static ConfigTicket GetTicket(this HttpRequest request, string scheme = "acb")
        {
            if (!request.Headers.TryGetValue("Authorization", out var authorize) ||
                string.IsNullOrWhiteSpace(authorize))
                return null;
            var arr = authorize.ToString()?.Split(' ');
            if (arr == null || arr.Length != 2 || arr[0] != scheme)
                return null;
            var ticket = arr[1];
            return ticket.Client<ConfigTicket>();
        }
    }
}
