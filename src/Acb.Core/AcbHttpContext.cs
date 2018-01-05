using Acb.Core.Helper;
using Microsoft.AspNetCore.Http;
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

        public static HttpContext Current => _accessor?.HttpContext;

        /// <summary>
        /// 客户端IP
        /// </summary>
        public static string RemoteIpAddress
        {
            get
            {
                var context = Current;
                if (context == null)
                    return DefaultIp;
                //获取代理IP
                context.Request.Headers.TryGetValue("HTTP_X_FORWARDED_FOR", out var userHostAddress);
                if (string.IsNullOrWhiteSpace(userHostAddress))
                {
                    context.Request.Headers.TryGetValue("X-Forwarded-For", out userHostAddress);
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
        /// <summary>
        /// 本地IP
        /// </summary>
        public static string LocalIpAddress
        {
            get
            {
                return Current == null ? DefaultIp : Current.Connection.LocalIpAddress.ToString();
            }
        }
        /// <summary>
        /// 请求类型
        /// </summary>
        public static string RequestType => Current.Request.Method;

        /// <summary>
        /// 表单
        /// </summary>
        public static IFormCollection Form => Current.Request.Form;
        /// <summary>
        /// 请求体
        /// </summary>
        public static Stream Body => Current.Request.Body;

        /// <summary>
        /// 用户代理
        /// </summary>
        public static string UserAgent => Current.Request.Headers["User-Agent"];

        /// <summary>
        /// 内容类型
        /// </summary>
        public static string ContentType => Current.Request.ContentType;

        /// <summary>
        /// 参数
        /// </summary>
        public static string QueryString => Current.Request.QueryString.ToString();

        /// <summary> 配置HttpContext </summary>
        /// <param name="accessor"></param>
        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        public static string ClientIp => RemoteIpAddress;
    }
}
