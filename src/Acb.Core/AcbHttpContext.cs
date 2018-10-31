using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace Acb.Core
{
    /// <summary>
    /// 模拟 HttpContext
    /// </summary>
    public static class AcbHttpContext
    {
        private static IHttpContextAccessor _accessor;
        private const string DefaultIp = "127.0.0.1";

        /// <summary> 当前请求上下文 </summary>
        public static HttpContext Current => _accessor?.HttpContext;

        /// <summary> 客户端IP </summary>
        public static string RemoteIpAddress
        {
            get
            {
                var context = Current;
                if (context == null)
                    return DefaultIp;

                string GetIpFromHeader(string key)
                {

                    if (!context.Request.Headers.TryGetValue(key, out var ips))
                        return string.Empty;
                    foreach (var ip in ips)
                    {
                        if (RegexHelper.IsIp(ip)) return ip;
                    }

                    return string.Empty;
                }
                //获取代理IP
                var userHostAddress = GetIpFromHeader("X-Forwarded-For");
                if (!string.IsNullOrWhiteSpace(userHostAddress))
                    return userHostAddress;
                userHostAddress = GetIpFromHeader("X-Real-IP");
                if (!string.IsNullOrWhiteSpace(userHostAddress))
                    return userHostAddress;
                userHostAddress = GetIpFromHeader("HTTP_X_FORWARDED_FOR");
                if (!string.IsNullOrWhiteSpace(userHostAddress))
                    return userHostAddress;
                userHostAddress = GetIpFromHeader("REMOTE_ADDR");
                if (!string.IsNullOrWhiteSpace(userHostAddress))
                    return userHostAddress;
                userHostAddress = context.Connection.RemoteIpAddress.ToString();
                return RegexHelper.IsIp(userHostAddress) ? userHostAddress : DefaultIp;
            }
        }
        /// <summary> 本地IP </summary>
        public static string LocalIpAddress
        {
            get
            {
                if (Current == null)
                    return DefaultIp;
                var ip = Current.Connection.LocalIpAddress.ToString();
                if (string.IsNullOrEmpty(ip) || !RegexHelper.IsIp(ip))
                {
                    return DefaultIp;
                }

                return ip;
            }
        }
        /// <summary> 请求类型 </summary>
        public static string RequestType => Current.Request.Method;

        /// <summary> 表单 </summary>
        public static IFormCollection Form => Current.Request.Form;

        /// <summary> 请求体 </summary>
        public static Stream Body => Current.Request.Body;

        /// <summary> 用户代理 </summary>
        public static string UserAgent => Current?.Request.Headers["User-Agent"];

        /// <summary> 内容类型 </summary>
        public static string ContentType => Current.Request.ContentType;

        /// <summary> 参数 </summary>
        public static string QueryString => Current.Request.QueryString.ToString();

        public static string RawUrl => Utils.RawUrl(Current?.Request);

        /// <summary> 配置HttpContext </summary>
        /// <param name="accessor"></param>
        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary> 获取客户端IP </summary>
        public static string ClientIp => RemoteIpAddress;

        public static T FromBody<T>()
        {
            if (Current == null) return default(T);

            string GetBody()
            {
                if (Body.CanSeek)
                {
                    Body.Seek(0, SeekOrigin.Begin);
                }

                using (var stream = new StreamReader(Body))
                {
                    return stream.ReadToEnd();
                }
            }

            string body;

            switch (ContentType)
            {
                case "application/x-www-form-urlencoded":
                    return Form.FromForm().ToObject<T>();
                case "application/xml":
                case "text/xml":
                    body = GetBody();
                    var dict = new Dictionary<string, object>();
                    dict.FromXml(body);
                    return dict.ToObject<T>();
                default:
                    body = GetBody();
                    return JsonHelper.Json<T>(body, NamingType.CamelCase);
            }
        }
    }
}
