using Acb.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Configuration
{
    public static class HttpClientHelper
    {
        internal static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ReflectedDelegate;
        //private const int DEFAULT_GETACCESSTOKEN_TIMEOUT = 10000;
        //private const bool DEFAULT_VALIDATE_CERTIFICATES = true;

        public const string NetFramework = ".NET Framework";
        public const string NetCore = ".NET Core";

        public static bool IsFullFramework => RuntimeInformation.FrameworkDescription.StartsWith(NetFramework);

        public static bool IsNetCore => RuntimeInformation.FrameworkDescription.StartsWith(NetCore);
        private static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> DefaultDelegate { get; } = (Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>)((sender, cert, chain, sslPolicyErrors) => true);

        public static HttpClient GetHttpClient(bool validateCertificates, int timeout)
        {
            HttpClient httpClient;
            if (IsFullFramework)
                httpClient = new HttpClient();
            else if (!validateCertificates)
                httpClient = new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = GetDisableDelegate(),
                    SslProtocols = SslProtocols.Tls12
                });
            else
                httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
            return httpClient;
        }

        public static void ConfigureCertificateValidatation(bool validateCertificates, out SecurityProtocolType protocolType, out RemoteCertificateValidationCallback prevValidator)
        {
            prevValidator = null;
            protocolType = 0;
            if (!IsFullFramework || validateCertificates)
                return;
            protocolType = ServicePointManager.SecurityProtocol;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            prevValidator = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        }

        public static void RestoreCertificateValidation(bool validateCertificates, SecurityProtocolType protocolType, RemoteCertificateValidationCallback prevValidator)
        {
            if (!IsFullFramework || validateCertificates)
                return;
            ServicePointManager.SecurityProtocol = protocolType;
            ServicePointManager.ServerCertificateValidationCallback = prevValidator;
        }

        public static string GetEncodedUserPassword(string user, string password)
        {
            if (user == null)
                user = string.Empty;
            if (password == null)
                password = string.Empty;
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        }

        public static HttpRequestMessage GetRequestMessage(HttpMethod method, string requestUri, Func<string> getAccessToken)
        {
            var requestMessage = HttpClientHelper.GetRequestMessage(method, requestUri, (string)null, (string)null);
            var parameter = getAccessToken?.Invoke();
            if (parameter == null) return requestMessage;
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", parameter);
            requestMessage.Headers.Authorization = authenticationHeaderValue;
            return requestMessage;
        }

        public static HttpRequestMessage GetRequestMessage(HttpMethod method, string requestUri, string userName, string password)
        {
            if (method == (HttpMethod)null)
                throw new ArgumentNullException(nameof(method));
            if (requestUri == null)
                throw new ArgumentNullException(nameof(requestUri));
            var httpRequestMessage = new HttpRequestMessage(method, requestUri);
            if (string.IsNullOrEmpty(password)) return httpRequestMessage;
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", GetEncodedUserPassword(userName, password));
            httpRequestMessage.Headers.Authorization = authenticationHeaderValue;
            return httpRequestMessage;
        }

        public static async Task<string> GetAccessToken(string accessTokenUri, string clientId, string clientSecret, int timeout = 10000, bool validateCertificates = true, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(accessTokenUri))
                throw new ArgumentException(nameof(accessTokenUri));
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException(nameof(accessTokenUri));
            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException(nameof(accessTokenUri));
            var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUri);
            var client = GetHttpClient(validateCertificates, timeout);
            ConfigureCertificateValidatation(validateCertificates, out var prevProtocols, out var prevValidator);
            var auth = new AuthenticationHeaderValue("Basic", GetEncodedUserPassword(clientId, clientSecret));
            request.Headers.Authorization = auth;
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {
                    "grant_type",
                    "client_credentials"
                }
            });

            try
            {
                HttpClient httpClient = client;
                try
                {
                    HttpResponseMessage httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                    HttpResponseMessage response = httpResponseMessage;
                    try
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            logger?.Info(
                                "GetAccessToken returned status: {0} while obtaining access token from: {1}",
                                response.StatusCode as object, accessTokenUri as object);
                            return null;
                        }

                        var json = await response.Content.ReadAsStringAsync();
                        var payload = JObject.Parse(json);
                        var token = payload.Value<string>("access_token");
                        return token;
                    }
                    finally
                    {
                        response?.Dispose();
                    }
                }
                finally
                {
                    httpClient?.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger?.Error("GetAccessToken exception: {0} ,obtaining access token from: {1}", new object[]
                {
                    ex,
                    accessTokenUri
                });
            }
            finally
            {
                RestoreCertificateValidation(validateCertificates, prevProtocols, prevValidator);
            }

            return null;
        }

        internal static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> GetDisableDelegate()
        {
            if (IsFullFramework)
                return null;
            if (ReflectedDelegate != null)
                return ReflectedDelegate;
            var property = typeof(HttpClientHandler).GetProperty("DangerousAcceptAnyServerCertificateValidator", BindingFlags.Static | BindingFlags.Public);
            if (property == null) return DefaultDelegate;
            ReflectedDelegate =
                property.GetValue(null) as
                    Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>;
            return ReflectedDelegate ?? DefaultDelegate;
        }
    }
}
