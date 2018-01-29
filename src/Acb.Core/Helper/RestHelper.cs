using Acb.Core.Exceptions;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acb.Core.Helper
{
    public class RestHelper
    {
        private readonly string _baseUri;
        private readonly ILogger _logger = LogManager.Logger<RestHelper>();

        public RestHelper(string baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary> 请求接口 </summary>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> RequestAsync(string api, object paras = null, object data = null,
            HttpMethod method = null, object headers = null, HttpContent content = null)
        {
            var uri = new Uri(new Uri(_baseUri), api);

            var resp = await HttpHelper.Instance.RequestAsync(method ?? HttpMethod.Get, uri.AbsoluteUri, paras, data,
                headers, content);
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new BusiException("接口请求异常");
            return await resp.Content.ReadAsStringAsync();
        }

        /// <summary> 获取API接口返回的实体对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string api, object paras = null, object data = null,
            HttpMethod method = null, object headers = null, HttpContent content = null)
            where T : DResult, new()
        {
            var html = await RequestAsync(api, paras, data, method, headers, content);
            try
            {
                if (string.IsNullOrWhiteSpace(html))
                    return new T { Code = -1, Message = "无数据" };
                var setting = new JsonSerializerSettings();
                setting.Converters.Add(new DateTimeConverter());
                return JsonConvert.DeserializeObject<T>(html, setting);
                //return JsonHelper.Json<T>(html);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new T { Code = -1, Message = "服务器数据异常" };
            }
        }

        /// <summary> 请求接口 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<DResult<T>> ResultAsync<T>(string api, object paras = null, object data = null,
            HttpMethod method = null, object headers = null, HttpContent content = null)
        {
            return await RequestAsync<DResult<T>>(api, paras, data, method, headers, content);
        }

        /// <summary> 获取API接口返回的实体对象 </summary>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<DResult> ResultAsync(string api, object paras = null, object data = null,
            HttpMethod method = null, object headers = null, HttpContent content = null)
        {
            return await RequestAsync<DResult>(api, paras, data, method, headers, content);
        }

        public async Task<T> GetAsync<T>(string api, object paras = null, object headers = null)
            where T : DResult, new() => await RequestAsync<T>(api, paras, null, HttpMethod.Get, headers);

        public async Task<T> PostAsync<T>(string api, object data = null, object paras = null,
            object headers = null, HttpContent content = null)
            where T : DResult, new() => await RequestAsync<T>(api, paras, data, HttpMethod.Post, headers, content);

        public async Task<T> PutAsync<T>(string api, object data = null, object paras = null,
            object headers = null, HttpContent content = null)
            where T : DResult, new() => await RequestAsync<T>(api, paras, data, HttpMethod.Put, headers, content);

        public async Task<T> DeleteAsync<T>(string api, object paras = null, object headers = null)
            where T : DResult, new() => await RequestAsync<T>(api, paras, null, HttpMethod.Delete, headers);
    }
}
