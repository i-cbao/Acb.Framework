using Acb.Core.Exceptions;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Acb.Core.Helper;

namespace Acb.Configuration
{
    public class ConfigServerConfigurationProvider : ConfigurationProvider, IConfigurationSource
    {
        public const string Prefix = "config";

        public const string TokenHeader = "X-Config-Token";
        public const string StateHeader = "X-Config-State";

        private readonly ConfigServerClientSettings _settings;
        private HttpClient _client;

        private const string ArrayPattern = @"(\[[0-9]+\])*$";
        private static readonly char[] CommaDelimit = { ',' };
        private static readonly string[] EmptyLabels = { string.Empty };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigServerConfigurationProvider"/> class with default
        /// configuration settings. <see cref="ConfigServerClientSettings"/>
        /// </summary>
        public ConfigServerConfigurationProvider()
            : this(new ConfigServerClientSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigServerConfigurationProvider"/> class.
        /// </summary>
        /// <param name="settings">the configuration settings the provider uses when accessing the server.</param>
        public ConfigServerConfigurationProvider(ConfigServerClientSettings settings)
        {
            Logger = LogManager.Logger<ConfigServerConfigurationProvider>();
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _client = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigServerConfigurationProvider"/> class.
        /// </summary>
        /// <param name="settings">the configuration settings the provider uses when accessing the server.</param>
        /// <param name="httpClient">a HttpClient the provider uses to make requests of the server.</param>
        public ConfigServerConfigurationProvider(ConfigServerClientSettings settings, HttpClient httpClient)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Logger = LogManager.Logger<ConfigServerConfigurationProvider>();
        }

        /// <summary>
        /// Gets the configuration settings the provider uses when accessing the server.
        /// </summary>
        public virtual ConfigServerClientSettings Settings => _settings;

        internal IDictionary<string, string> Properties => Data;

        internal ILogger Logger { get; }

        /// <summary>
        /// Loads configuration data from the Spring Cloud Configuration Server as specified by
        /// the <see cref="Settings"/>
        /// </summary>
        public override void Load()
        {
            // Adds client settings (e.g spring:cloud:config:uri, etc) to the Data dictionary
            AddConfigServerClientSettings();
            if (_settings.RetryEnabled && _settings.FailFast)
            {
                var attempts = 0;
                var backOff = _settings.RetryInitialInterval;
                do
                {
                    Logger?.Info("Fetching config from server at: {0}", _settings.Uri);
                    try
                    {
                        DoLoad();
                        return;
                    }
                    catch (Exception e)
                    {
                        Logger?.Info("Failed fetching config from server at: {0}, Exception: {1}", _settings.Uri, e);
                        attempts++;
                        if (attempts < _settings.RetryAttempts)
                        {
                            Thread.CurrentThread.Join(backOff);
                            var nextBackoff = (int)(backOff * _settings.RetryMultiplier);
                            backOff = Math.Min(nextBackoff, _settings.RetryMaxInterval);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                while (true);
            }
            Logger?.Info("Fetching config from server at: {0}", _settings.Uri);
            DoLoad();
        }

        public virtual IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            ConfigurationSettingsHelper.Initialize(Prefix, _settings, ConfigHelper.Instance.Config);
            return this;
        }

        internal void DoLoad()
        {
            var names = _settings.Name.Split(CommaDelimit, StringSplitOptions.RemoveEmptyEntries);
            var labels = GetLabels();
            foreach (var name in names)
            {
                try
                {
                    foreach (var label in labels)
                    {
                        // Make Config Server URI from settings
                        var path = GetConfigServerUri(name, label);

                        // Invoke config server, and wait for results
                        var task = RemoteLoadAsync(path);
                        task.Wait();
                        var env = task.Result;

                        // Update config Data dictionary with any results
                        if (env == null)
                            continue;
                        Logger?.Info("Located environment: {0}, {1}, {2}, {3}", env.Name,
                            JsonHelper.ToJson(env.Profiles), env.Label, env.Version);
                        var sources = env.PropertySources;
                        if (sources == null)
                            continue;
                        var index = sources.Count - 1;
                        for (; index >= 0; index--)
                        {
                            AddPropertySource(sources[index]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger?.Warn("Could not locate PropertySource: " + e);

                    if (_settings.FailFast)
                    {
                        throw new BusiException("Could not locate PropertySource, fail fast property is set, failing");
                    }
                }
            }
        }

        internal void Reload(IConfiguration config)
        {
            ConfigurationSettingsHelper.Initialize(Prefix, _settings, config);
            Load();
        }

        internal string[] GetLabels()
        {
            if (string.IsNullOrWhiteSpace(_settings.Label))
            {
                return EmptyLabels;
            }

            return _settings.Label.Split(CommaDelimit, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Create the HttpRequestMessage that will be used in accessing the Spring Cloud Configuration server
        /// </summary>
        /// <param name="requestUri">the Uri used when accessing the server</param>
        /// <returns>The HttpRequestMessage built from the path</returns>
        protected internal virtual HttpRequestMessage GetRequestMessage(string requestUri)
        {
            var request =
                HttpClientHelper.GetRequestMessage(HttpMethod.Get, requestUri, _settings.Username, _settings.Password);

            if (!string.IsNullOrEmpty(_settings.Token))
            {
                RenewToken(_settings.Token);
                request.Headers.Add(TokenHeader, _settings.Token);
            }

            return request;
        }

        /// <summary>
        /// Adds the client settings for the Configuration Server to the data dictionary
        /// </summary>
        protected internal virtual void AddConfigServerClientSettings()
        {
            Data[$"{Prefix}:enabled"] = _settings.Enabled.ToString();
            Data[$"{Prefix}:failFast"] = _settings.FailFast.ToString();
            Data[$"{Prefix}:env"] = _settings.Environment;
            Data[$"{Prefix}:label"] = _settings.Label;
            Data[$"{Prefix}:name"] = _settings.Name;
            Data[$"{Prefix}:password"] = _settings.Password;
            Data[$"{Prefix}:uri"] = _settings.Uri;
            Data[$"{Prefix}:username"] = _settings.Username;
            Data[$"{Prefix}:token"] = _settings.Token;
            Data[$"{Prefix}:timeout"] = _settings.Timeout.ToString();
            Data[$"{Prefix}:validate_certificates"] = _settings.ValidateCertificates.ToString();
            Data[$"{Prefix}:retry:enabled"] = _settings.RetryEnabled.ToString();
            Data[$"{Prefix}:retry:maxAttempts"] = _settings.RetryAttempts.ToString();
            Data[$"{Prefix}:retry:initialInterval"] = _settings.RetryInitialInterval.ToString();
            Data[$"{Prefix}:retry:maxInterval"] = _settings.RetryMaxInterval.ToString();
            Data[$"{Prefix}:retry:multiplier"] = _settings.RetryMultiplier.ToString();
        }

        /// <summary>
        /// Asynchronously calls the Spring Cloud Configuration Server using the provided Uri and returning a
        /// a task that can be used to obtain the results
        /// </summary>
        /// <param name="requestUri">the Uri used in accessing the Spring Cloud Configuration Server</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        protected internal virtual async Task<ConfigEnvironment> RemoteLoadAsync(string requestUri)
        {
            // Get client if not already set
            if (_client == null)
            {
                _client = GetHttpClient(_settings);
            }

            // Get the request message
            var request = GetRequestMessage(requestUri);

            // If certificate validation is disabled, inject a callback to handle properly
            HttpClientHelper.ConfigureCertificateValidatation(_settings.ValidateCertificates, out var prevProtocols, out var prevValidator);

            // Invoke config server
            try
            {
                using (HttpResponseMessage response = await _client.SendAsync(request).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return null;
                        }

                        // Log status
                        var message = $"Config Server returned status: {response.StatusCode} invoking path: {requestUri}";

                        Logger?.Info(message);

                        // Throw if status >= 400
                        if (response.StatusCode >= HttpStatusCode.BadRequest)
                        {
                            throw new HttpRequestException(message);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    Stream stream = await response.Content.ReadAsStreamAsync();
                    return Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                // Log and rethrow
                Logger?.Error("Config Server exception: {0}, path: {1}", e, requestUri);
                throw;
            }
            finally
            {
                HttpClientHelper.RestoreCertificateValidation(_settings.ValidateCertificates, prevProtocols, prevValidator);
            }
        }

        /// <summary>
        /// Deserialize the response from the Configuration Server
        /// </summary>
        /// <param name="stream">the stream representing the response from the Configuration Server</param>
        /// <returns>The ConfigEnvironment object representing the response from the server</returns>
        protected internal virtual ConfigEnvironment Deserialize(Stream stream)
        {
            return SerializationHelper.Deserialize<ConfigEnvironment>(stream, Logger);
        }

        /// <summary>
        /// Create the Uri that will be used in accessing the Configuration Server
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label">a label to add</param>
        /// <returns>The request URI for the Configuration Server</returns>
        protected internal virtual string GetConfigServerUri(string name, string label)
        {
            var path = name + "/" + _settings.Environment;
            if (!string.IsNullOrWhiteSpace(label))
            {
                path = path + "/" + label;
            }

            if (!_settings.RawUri.EndsWith("/"))
            {
                path = "/" + path;
            }

            return _settings.RawUri + path;
        }

        /// <summary>
        /// Adds values from a PropertySource to the Configurtation Data dictionary managed
        /// by this provider
        /// </summary>
        /// <param name="source">a property source to add</param>
        protected internal virtual void AddPropertySource(PropertySource source)
        {
            if (source?.Source == null)
            {
                return;
            }

            foreach (var kvp in source.Source)
            {
                try
                {
                    var key = ConvertKey(kvp.Key);
                    var value = ConvertValue(kvp.Value);
                    Data[key] = value;
                }
                catch (Exception e)
                {
                    Logger?.Error("Config Server exception, property: {0}={1}", kvp.Key, kvp.Value.GetType(), e);
                }
            }
        }

        protected internal virtual string ConvertKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            var split = key.Split('.');
            var sb = new StringBuilder();
            foreach (var part in split)
            {
                var keyPart = ConvertArrayKey(part);
                sb.Append(keyPart);
                sb.Append(ConfigurationPath.KeyDelimiter);
            }

            return sb.ToString(0, sb.Length - 1);
        }

        protected internal virtual string ConvertArrayKey(string key)
        {
            return Regex.Replace(key, ArrayPattern, (match) =>
            {
                var result = match.Value.Replace("[", ":").Replace("]", string.Empty);
                return result;
            });
        }

        protected internal virtual string ConvertValue(object value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Encode the username password for a http request
        /// </summary>
        /// <param name="user">the username</param>
        /// <param name="password">the password</param>
        /// <returns>Encoded user + password</returns>
        protected internal string GetEncoded(string user, string password)
        {
            return HttpClientHelper.GetEncodedUserPassword(user, password);
        }

        protected internal virtual void RenewToken(string token)
        {
        }

        /// <summary>
        /// Creates an appropriatly configured HttpClient that will be used in communicating with the
        /// Spring Cloud Configuration Server
        /// </summary>
        /// <param name="settings">the settings used in configuring the HttpClient</param>
        /// <returns>The HttpClient used by the provider</returns>
        protected static HttpClient GetHttpClient(ConfigServerClientSettings settings)
        {
            return HttpClientHelper.GetHttpClient(settings.ValidateCertificates, settings.Timeout);
        }
    }
}
