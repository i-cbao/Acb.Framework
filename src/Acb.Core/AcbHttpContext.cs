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
                //获取代理IP
                context.Request.Headers.TryGetValue("X-Forwarded-For", out var userHostAddress);
                if (string.IsNullOrWhiteSpace(userHostAddress))
                {
                    context.Request.Headers.TryGetValue("HTTP_X_FORWARDED_FOR", out userHostAddress);
                }
                if (string.IsNullOrEmpty(userHostAddress))
                {
                    context.Request.Headers.TryGetValue("REMOTE_ADDR", out userHostAddress);
                }

                if (string.IsNullOrEmpty(userHostAddress))
                {
                    userHostAddress = context.Connection.RemoteIpAddress.ToString();
                }

                if (string.IsNullOrEmpty(userHostAddress) || !RegexHelper.IsIp(userHostAddress))
                {
                    return DefaultIp;
                }

                return userHostAddress;
            }
        }
        /// <summary> 本地IP </summary>
        public static string LocalIpAddress
        {
            get
            {
                return Current == null ? DefaultIp : Current.Connection.LocalIpAddress.ToString();
            }
        }
        /// <summary> 请求类型 </summary>
        public static string RequestType => Current.Request.Method;

        /// <summary> 表单 </summary>
        public static IFormCollection Form => Current.Request.Form;

        /// <summary> 请求体 </summary>
        public static Stream Body => Current.Request.Body;

        /// <summary> 用户代理 </summary>
        public static string UserAgent => Current.Request.Headers["User-Agent"];

        /// <summary> 内容类型 </summary>
        public static string ContentType => Current.Request.ContentType;

        /// <summary> 参数 </summary>
        public static string QueryString => Current.Request.QueryString.ToString();

        public static string RawUrl => Utils.RawUrl();

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
