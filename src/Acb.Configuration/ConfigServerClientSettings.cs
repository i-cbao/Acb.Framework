using System;

namespace Acb.Configuration
{
    /// <summary>
    /// Holds the settings used to configure the Spring Cloud Config Server provider
    /// <see cref="ConfigServerConfigurationProvider"/>.
    /// </summary>
    public class ConfigServerClientSettings
    {
        /// <summary>
        /// Default fail fast setting
        /// </summary>
        public const bool DefaultFailfast = false;

        /// <summary>
        /// Default Config Server provider enabled setting
        /// </summary>
        public const bool DefaultProviderEnabled = true;

        /// <summary>
        /// Default certifcate validation enabled setting
        /// </summary>
        public const bool DefaultCertificateValidation = true;

        /// <summary>
        /// Default number of retries to be attempted
        /// </summary>
        public const int DefaultMaxRetryAttempts = 6;

        /// <summary>
        /// Default initial retry interval in milliseconds
        /// </summary>
        public const int DefaultInitialRetryInterval = 1000;

        /// <summary>
        /// Default multiplier for next retry interval
        /// </summary>
        public const double DefaultRetryMultiplier = 1.1;

        /// <summary>
        /// Default initial retry interval in milliseconds
        /// </summary>
        public const int DefaultMaxRetryInterval = 2000;

        /// <summary>
        /// Default retry enabled setting
        /// </summary>
        public const bool DefaultRetryEnabled = false;

        /// <summary>
        /// Default timeout in milliseconds
        /// </summary>
        public const int DefaultTimeoutMilliseconds = 6 * 1000;

        /// <summary> 默认刷新时间(s) </summary>
        public const int DefaultRefreshInterval = 5 * 60;

        private static readonly char[] ColonDelimit = { ':' };
        private string _username;
        private string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigServerClientSettings"/> class.
        /// </summary>
        /// <remarks>Initialize Config Server client settings with defaults</remarks>
        public ConfigServerClientSettings()
            : base()
        {
            ValidateCertificates = DefaultCertificateValidation;
            FailFast = DefaultFailfast;
            Enabled = DefaultProviderEnabled;
            RetryEnabled = DefaultRetryEnabled;
            RetryInitialInterval = DefaultInitialRetryInterval;
            RetryMaxInterval = DefaultMaxRetryInterval;
            RetryAttempts = DefaultMaxRetryAttempts;
            RetryMultiplier = DefaultRetryMultiplier;
            Timeout = DefaultTimeoutMilliseconds;
            RefreshInterval = DefaultRefreshInterval;
        }

        /// <summary>
        /// Gets or sets the Config Server address
        /// </summary>
        public virtual string Uri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enables/Disables the Config Server provider
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the environment used when accessing configuration data
        /// </summary>
        public virtual string Environment { get; set; }

        /// <summary>
        /// Gets or sets the application name used when accessing configuration data
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the label used when accessing configuration data
        /// </summary>
        public virtual string Label { get; set; }

        /// <summary>
        /// Gets or sets the username used when accessing the Config Server
        /// </summary>
        public virtual string Username
        {
            get { return GetUserName(); }
            set { this._username = value; }
        }

        /// <summary>
        /// Gets or sets the password used when accessing the Config Server
        /// </summary>
        public virtual string Password
        {
            get { return GetPassword(); }
            set { this._password = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enables/Disables failfast behavior
        /// </summary>
        public virtual bool FailFast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enables/Disables whether provider validates server certificates
        /// </summary>
        public virtual bool ValidateCertificates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enables/Disables config server client retry on failures
        /// </summary>
        public virtual bool RetryEnabled { get; set; }

        /// <summary>
        /// Gets or sets initial retry interval in milliseconds
        /// </summary>
        public virtual int RetryInitialInterval { get; set; }

        /// <summary>
        /// Gets or sets max retry interval in milliseconds
        /// </summary>
        public virtual int RetryMaxInterval { get; set; }

        /// <summary>
        ///  Gets or sets multiplier for next retry interval
        /// </summary>
        public virtual double RetryMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the max number of retries the client will attempt
        /// </summary>
        public virtual int RetryAttempts { get; set; }

        /// <summary>
        /// Gets returns the HttpRequestUrl, unescaped
        /// </summary>
        public virtual string RawUri
        {
            get { return GetRawUri(); }
        }

        /// <summary>
        /// Gets or sets returns the token use for Vault
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets returns the request timeout in milliseconds
        /// </summary>
        public virtual int Timeout { get; set; }

        /// <summary> 开启刷新 </summary>
        public virtual bool RefreshEnable { get; set; }

        /// <summary> 刷新周期(秒) </summary>
        public virtual int RefreshInterval { get; set; }

        internal string GetRawUri()
        {
            try
            {
                if (!string.IsNullOrEmpty(Uri))
                {
                    System.Uri uri = new System.Uri(Uri);
                    return uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
                }
            }
            catch (UriFormatException)
            {
            }

            return Uri;
        }

        internal string GetPassword()
        {
            if (!string.IsNullOrEmpty(_password))
            {
                return _password;
            }

            return GetUserPassElement(1);
        }

        internal string GetUserName()
        {
            if (!string.IsNullOrEmpty(_username))
            {
                return _username;
            }

            return GetUserPassElement(0);
        }

        private string GetUserInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(Uri))
                {
                    System.Uri uri = new System.Uri(Uri);
                    return uri.UserInfo;
                }
            }
            catch (UriFormatException)
            {
                // Log
                throw;
            }

            return null;
        }

        private string GetUserPassElement(int index)
        {
            string result = null;
            string userInfo = GetUserInfo();
            if (!string.IsNullOrEmpty(userInfo))
            {
                string[] info = userInfo.Split(ColonDelimit);
                if (info.Length > index)
                {
                    result = info[index];
                }
            }

            return result;
        }
    }
}
