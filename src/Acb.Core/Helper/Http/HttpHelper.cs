using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Core.Helper.Http
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
            lock (Client)
            {
                foreach (var header in DefaultHeaders)
                {
                    Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                Client.Timeout = TimeSpan.FromSeconds(65);
            }
        }

        /// <summary> 单例 </summary>
        public static HttpHelper Instance =>
            Singleton<HttpHelper>.Instance ?? (Singleton<HttpHelper>.Instance = new HttpHelper());

        /// <summary> 设置超时时间 </summary>
        /// <param name="timeout"></param>
        public void Timeout(TimeSpan timeout)
        {
            Client.Timeout = timeout;
        }

        /// <summary> 请求 </summary>
        /// <param name="method"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RequestAsync(HttpMethod method, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Url))
                throw new BusiException("Http请求参数异常");
            request.Encoding = request.Encoding ?? Encoding.UTF8;
            request.Headers = request.Headers ?? new Dictionary<string, string>();

            var url = request.Url;
            if (request.Params != null)
            {
                url += url.IndexOf('?') > 0 ? "&" : "?";
                url += request.Params.ToDictionary().ToUrl(encoding: request.Encoding);
            }

            var uri = new Uri(url);

            var req = new HttpRequestMessage(method, uri);
            if (request.Headers != null)
            {
                foreach (var key in request.Headers)
                {
                    if (string.IsNullOrWhiteSpace(key.Value))
                        continue;
                    req.Headers.TryAddWithoutValidation(key.Key, key.Value);
                }
            }
            HttpContent content = null;
            if (request.Data != null && method != HttpMethod.Get)
            {
                switch (request.BodyType)
                {
                    case HttpBodyType.Json:
                        content = new StringContent(JsonHelper.ToJson(request.Data), request.Encoding,
                            "application/json");
                        break;
                    case HttpBodyType.Form:
                        var str = string.Empty;
                        if (request.Data != null)
                        {
                            var type = request.Data.GetType();
                            if (type.IsSimpleType())
                            {
                                str = request.Data.ToString();
                            }
                            else
                            {
                                var dict = request.Data.ToDictionary();
                                str = dict.ToUrl(true, request.Encoding);
                            }
                        }
                        content = new ByteArrayContent(request.Encoding.GetBytes(str));
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        break;
                    case HttpBodyType.Xml:
                        var xmlDict = request.Data.ToDictionary();
                        content = new StringContent(xmlDict.ToXml(), request.Encoding, "text/xml");
                        break;
                    case HttpBodyType.File:
                        if (request.Files != null && request.Files.Any())
                        {
                            var multiContent = new MultipartFormDataContent(DateTime.Now.Ticks.ToString("x"));
                            foreach (var file in request.Files)
                            {
                                var stream = new MemoryStream();
                                var buffer = new byte[checked((uint)Math.Min(4096, (int)file.Value.Length))];
                                int bytesRead;
                                while ((bytesRead = file.Value.Read(buffer, 0, buffer.Length)) != 0)
                                    stream.Write(buffer, 0, bytesRead);
                                multiContent.Add(new StreamContent(stream), file.Key, file.Value.Name);
                            }
                            content = multiContent;
                        }
                        break;
                }
            }
            if (content != null)
                req.Content = content;
            var formData = request.Data == null ? string.Empty : "->" + JsonHelper.ToJson(request.Data);
            Logger.Debug($"HttpHelper：[{method}]{url}{formData}");
            return await Client.SendAsync(req);
        }

        /// <summary> Get方法 </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(HttpRequest request)
        {
            return await RequestAsync(HttpMethod.Get, request);
        }

        /// <summary> Get方法 </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, object param = null)
        {
            return await RequestAsync(HttpMethod.Get, new HttpRequest(url) { Params = param });
        }

        /// <summary> Post方法 </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(HttpRequest request)
        {
            return await RequestAsync(HttpMethod.Post, request);
        }

        /// <summary> Post方法 </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, object data = null)
        {
            return await RequestAsync(HttpMethod.Post, new HttpRequest(url) { Data = data });
        }

        /// <summary> Post方法 </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> FormPostAsync(string url, object data = null)
        {
            return await RequestAsync(HttpMethod.Post,
                new HttpRequest(url) { Data = data, BodyType = HttpBodyType.Form });
        }

        /// <summary> Post方法 </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> XmlPostAsync(string url, object data = null)
        {
            return await RequestAsync(HttpMethod.Post,
                new HttpRequest(url) { Data = data, BodyType = HttpBodyType.Xml });
        }

        /// <inheritdoc />
        /// <summary> 释放资源 </summary>
        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
