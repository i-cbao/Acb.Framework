using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Core.Helper
{
    /// <inheritdoc />
    /// <summary> Http请求类 </summary>
    public class HttpHelper : IDisposable
    {
        private static readonly HttpClient Client = new HttpClient();
        private static readonly ILogger Logger = LogManager.Logger<HttpHelper>();

        private static readonly IDictionary<string, string> DefaultHeaders = new Dictionary<string, string>
        {
            {"Accept-language", "zh-cn,zh;q=0.5"},
            {"Accept-Charset", "utf-8;q=0.7,*;q=0.7"},
            //{"Accept-Encoding", "gzip, deflate"},
            {"Keep-Alive", "350"},
            {"x-requested-with", "XMLHttpRequest"}
        };

        private HttpHelper()
        {
            foreach (var header in DefaultHeaders)
            {
                Client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        /// <summary> 单例 </summary>
        public static HttpHelper Instance =>
            Singleton<HttpHelper>.Instance ?? (Singleton<HttpHelper>.Instance = new HttpHelper());

        /// <summary> 请求 </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string url, object param = null,
            object data = null, object headers = null, HttpContent content = null)
        {
            if (param != null)
            {
                url += url.IndexOf('?') > 0 ? "&" : "?";
                url += param.ToDictionary().ToUrl();
            }

            var uri = new Uri(url);

            var req = new HttpRequestMessage(method, uri);
            if (headers != null)
            {
                var dict = headers.ToDictionary();
                foreach (var key in dict)
                {
                    req.Headers.Add(key.Key, key.Value?.ToString());
                }
            }

            if (content == null && data != null)
            {
                content = new StringContent(JsonHelper.ToJson(data), Encoding.UTF8, "application/json");
            }

            if (content != null)
                req.Content = content;
            var formData = data == null ? string.Empty : "->" + data.ToDictionary().ToUrl(false);
            Logger.Info($"HttpHelper：[{method}]{url}{formData}");
            return await Client.SendAsync(req);
        }

        /// <summary> 释放资源 </summary>
        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
