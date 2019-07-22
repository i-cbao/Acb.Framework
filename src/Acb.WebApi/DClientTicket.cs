using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Security;
using Acb.Core.Serialize;
using Acb.Core.Timing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;

namespace Acb.WebApi
{
    /// <summary> 客户端令牌接口 </summary>
    public interface IClientTicket
    {
        /// <summary> 用户Id </summary>
        object UserId { get; set; }
        /// <summary> 租户Id </summary>
        object TenantId { get; set; }

        /// <summary> 用户名称 </summary>
        string Name { get; set; }

        /// <summary> 用户角色 </summary>
        string Role { get; set; }
        string Ticket { get; set; }
        /// <summary> 过期时间 </summary>
        DateTime? ExpiredTime { get; set; }

        /// <summary> 时间戳 </summary>
        DateTime Timestamp { get; set; }
    }

    public class ClientTicket : IClientTicket
    {
        public object UserId { get; set; }
        public object TenantId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public string Ticket { get; set; }

        /// <inheritdoc />
        /// <summary> 过期时间 </summary>
        public DateTime? ExpiredTime { get; set; }

        private DateTime _timestamp;

        /// <inheritdoc />
        /// <summary> 时间戳 </summary>
        public DateTime Timestamp
        {
            get => _timestamp == DateTime.MinValue ? Clock.Now : _timestamp;
            set => _timestamp = value;
        }

        public virtual string GenerateTicket()
        {
            return JsonHelper.ToJson(this).Md5();
        }
    }

    /// <inheritdoc />
    /// <summary> 默认客户端令牌 </summary>
    public class DClientTicket<T> : ClientTicket
    {
        private T _id;
        /// <summary> 用户ID </summary>
        public T Id
        {
            get => _id;
            set
            {
                UserId = value;
                _id = value;
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// 默认客户端令牌
    /// </summary>
    public class DClientTicket : DClientTicket<Guid>
    {
    }

    /// <summary> 客户端密钥辅助类 </summary>
    public static class ClientTicketHelper
    {
        private const string TicketEncodeKey = "@%d^#41&";
        private const string TicketEncodeIv = "%@D^d$2~";

        /// <summary> 获取凭证 </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static string Ticket(this ClientTicket ticket)
        {
            ticket.Ticket = ticket.GenerateTicket();

            var json = JsonHelper.ToJson(ticket);
            return EncryptHelper.SymmetricEncrypt($"{ticket.Ticket}_{json}", EncryptHelper.SymmetricFormat.DES,
                TicketEncodeKey, TicketEncodeIv);
        }

        /// <summary> 获取凭证信息 </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static TTicket Client<TTicket>(this string ticket) where TTicket : IClientTicket
        {
            if (string.IsNullOrWhiteSpace(ticket))
                return default(TTicket);
            var logger = LogManager.Logger(typeof(ClientTicketHelper));
            try
            {
                var str = EncryptHelper.SymmetricDecrypt(ticket, EncryptHelper.SymmetricFormat.DES, TicketEncodeKey,
                    TicketEncodeIv);
                if (string.IsNullOrWhiteSpace(str))
                    return default(TTicket);
                var list = str.Split('_');
                //fixed json字符串中包含下划线时报错
                if (list.Length < 2)
                    return default(TTicket);
                var json = str.Substring(list[0].Length + 1);
                var client = JsonHelper.Json<TTicket>(json);
                if (!string.Equals(list[0], client.Ticket, StringComparison.CurrentCultureIgnoreCase))
                {
                    logger.Warn($"client ticket not equal,{client.Ticket}:{list[0]}");
                    return default(TTicket);
                }

                var principal = AcbHttpContext.Current?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
                if (principal != null)
                {
                    var claims = new List<Claim>();
                    if (client.UserId != null)
                    {
                        claims.AddRange(new[]
                        {
                            new Claim(AcbClaimTypes.UserId, client.UserId?.ToString()),
                            new Claim(AcbClaimTypes.UserName, client.Name),
                            new Claim(AcbClaimTypes.Role, client.Role)
                        });
                    }
                    if (client.TenantId != null)
                        claims.Add(new Claim(AcbClaimTypes.TenantId, client.TenantId.ToString()));
                    if (claims.Any())
                    {
                        var identity = new ClaimsIdentity(claims);
                        principal.AddIdentity(identity);
                    }
                }
                return client;

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return default(TTicket);
            }
        }

        /// <summary> 验证令牌 </summary>
        /// <param name="request"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static TTicket VerifyTicket<TTicket>(this HttpRequest request, string scheme = "acb")
            where TTicket : IClientTicket
        {
            if (!request.Headers.TryGetValue("Authorization", out var authorize) ||
                string.IsNullOrWhiteSpace(authorize))
                return default(TTicket);
            var arr = authorize.ToString().Split(' ');
            if (arr.Length != 2)
                return default(TTicket);
            return new AuthenticationHeaderValue(arr[0], arr[1]).VerifyTicket<TTicket>(scheme);
        }

        /// <summary> 验证令牌 </summary>
        /// <param name="authorize"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static TTicket VerifyTicket<TTicket>(this AuthenticationHeaderValue authorize, string scheme = "acb")
            where TTicket : IClientTicket
        {
            if (authorize == null ||
                !string.Equals(authorize.Scheme, scheme, StringComparison.CurrentCultureIgnoreCase))
                return default(TTicket);
            var ticket = authorize.Parameter;
            return ticket.Client<TTicket>();
        }
    }
}
