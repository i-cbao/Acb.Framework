﻿using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acb.Core.Helper.Http
{
    /// <summary> 接口调用辅助 </summary>
    public class RestHelper
    {
        private readonly string _baseUri;
        private const string Prefix = "sites:";
        private readonly ILogger _logger = LogManager.Logger<RestHelper>();

        /// <summary> 构造函数 </summary>
        /// <param name="baseUri"></param>
        public RestHelper(string baseUri = null)
        {
            _baseUri = baseUri;
        }

        /// <inheritdoc />
        /// <summary> 构造函数 </summary>
        /// <param name="siteEnum"></param>
        public RestHelper(Enum siteEnum) : this(
            $"{Prefix}{siteEnum.ToString().ToLower()}".Config<string>())
        {
        }

        private static string GetTicket()
        {
            var key = Consts.AppTicketKey.Config<string>();
            var timestamp = Clock.Now.ToTimestamp();
            return $"{timestamp}{EncryptHelper.Hash($"{key}{timestamp}", EncryptHelper.HashFormat.MD532).ToLower()}";
        }

        /// <summary> 请求接口 </summary>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public async Task<string> RequestAsync(HttpRequest request, HttpMethod method = null, bool ticket = false)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return string.Empty;
            if (!string.IsNullOrWhiteSpace(_baseUri))
                request.Url = string.Concat(_baseUri?.TrimEnd('/'), "/", request.Url.TrimStart('/'));

            request.Headers = request.Headers ?? new Dictionary<string, string>();
            if (ticket)
                request.Headers.Add("App-Ticket", GetTicket());
            var resp = await HttpHelper.Instance.RequestAsync(method ?? HttpMethod.Get, request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return await resp.Content.ReadAsStringAsync();
            return string.Empty;
        }

        /// <summary> 获取API接口返回的实体对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(HttpRequest request, HttpMethod method = null)
        {
            try
            {
                var html = await RequestAsync(request, method);
                if (!string.IsNullOrWhiteSpace(html))
                {
                    var setting = new JsonSerializerSettings();
                    setting.Converters.Add(new DateTimeConverter());
                    return JsonConvert.DeserializeObject<T>(html, setting);
                }
            }
            catch (Exception ex)
            {
                if (ex is BusiException)
                {
                    throw;
                }

                _logger.Error(ex.Message, ex);
            }
            return default(T);
        }

        /// <summary> GET </summary>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string api, object paras = null, IDictionary<string, string> headers = null)
            => await RequestAsync(new HttpRequest(api) { Params = paras, Headers = headers }, HttpMethod.Get);

        /// <summary> GET </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string api, object paras = null, IDictionary<string, string> headers = null)
            => await RequestAsync<T>(new HttpRequest(api) { Params = paras, Headers = headers }, HttpMethod.Get);

        /// <summary> POST </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(HttpRequest request) => await RequestAsync(request, HttpMethod.Post);

        /// <summary> POST </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, object data) =>
            await RequestAsync(new HttpRequest(url) { Data = data }, HttpMethod.Post);

        /// <summary> POST </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(HttpRequest request) => await RequestAsync<T>(request, HttpMethod.Post);

        /// <summary> POST </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, object data) =>
            await RequestAsync<T>(new HttpRequest(url) { Data = data }, HttpMethod.Post);

        /// <summary> PUT </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> PutAsync(string url, object param = null, object data = null) =>
            await RequestAsync(new HttpRequest(url) { Params = param, Data = data }, HttpMethod.Put);

        /// <summary> PUT </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> PutAsync(HttpRequest request) => await RequestAsync(request, HttpMethod.Put);

        /// <summary> PUT </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string url, object param = null, object data = null) =>
            await RequestAsync<T>(new HttpRequest(url) { Params = param, Data = data }, HttpMethod.Put);

        /// <summary> PUT </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(HttpRequest request) => await RequestAsync<T>(request, HttpMethod.Put);

        /// <summary> DELETE </summary>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(string api, object paras = null,
            IDictionary<string, string> headers = null)
            => await RequestAsync(new HttpRequest(api) { Params = paras, Headers = headers }, HttpMethod.Delete);

        /// <summary> DELETE </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(string api, object paras = null, IDictionary<string, string> headers = null)
            => await RequestAsync<T>(new HttpRequest(api)
            {
                Params = paras,
                Headers = headers
            }, HttpMethod.Delete);
    }
}
