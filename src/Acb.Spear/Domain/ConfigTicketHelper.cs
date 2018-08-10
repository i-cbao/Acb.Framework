using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.WebApi;
using Microsoft.AspNetCore.Http;

namespace Acb.Spear.Domain
{
    /// <summary> 项目凭证 </summary>
    public class ConfigTicket : ClientTicket
    {
        /// <summary> 项目编码 </summary>
        public string Code { get; set; }
    }
    public static class ConfigTicketHelper
    {
        private const string ProjectCodeKey = "project";
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
            var client = ticket.Client<ConfigTicket>();
            if (client.ExpiredTime.HasValue && client.ExpiredTime.Value < Clock.Now)
                return null;
            return client;
        }

        public static string GetProjectCode(this HttpRequest request)
        {
            var ticket = request.GetTicket();
            string code;
            if (ticket != null)
            {
                code = ticket.Code;
            }
            else
            {
                code = ProjectCodeKey.QueryOrForm(string.Empty);
                if (!string.IsNullOrWhiteSpace(code))
                    return code;
                if (request.Headers.TryGetValue(ProjectCodeKey, out var dcode))
                    code = dcode;
            }

            return code;
        }
    }
}
