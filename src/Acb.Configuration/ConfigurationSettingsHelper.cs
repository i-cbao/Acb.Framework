using Acb.Core;
using Acb.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System;

namespace Acb.Configuration
{
    public static class ConfigurationSettingsHelper
    {
        public static void Initialize(string configPrefix, ConfigServerClientSettings settings, IConfiguration config)
        {
            if (configPrefix == null)
            {
                throw new ArgumentNullException(nameof(configPrefix));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var clientConfigsection = config.GetSection(configPrefix);

            settings.Name = GetApplicationName(clientConfigsection, config, settings.Name);
            settings.Environment = Consts.Mode.ToString().ToLower(); /*config.GetValue<string>("mode")*/;
            settings.Label = GetLabel(clientConfigsection, config);
            settings.Username = GetUsername(clientConfigsection, config);
            settings.Password = GetPassword(clientConfigsection, config);
            settings.Uri = GetUri(clientConfigsection, config, settings.Uri);
            settings.Enabled = GetEnabled(clientConfigsection, config, settings.Enabled);
            settings.FailFast = GetFailFast(clientConfigsection, config, settings.FailFast);
            settings.ValidateCertificates = GetCertificateValidation(clientConfigsection, config, settings.ValidateCertificates);
            settings.RetryEnabled = GetRetryEnabled(clientConfigsection, config, settings.RetryEnabled);
            settings.RetryInitialInterval = GetRetryInitialInterval(clientConfigsection, config, settings.RetryInitialInterval);
            settings.RetryMaxInterval = GetRetryMaxInterval(clientConfigsection, config, settings.RetryMaxInterval);
            settings.RetryMultiplier = GetRetryMultiplier(clientConfigsection, config, settings.RetryMultiplier);
            settings.RetryAttempts = GetRetryMaxAttempts(clientConfigsection, config, settings.RetryAttempts);
            settings.Token = GetToken(clientConfigsection, config);
            settings.Timeout = GetTimeout(clientConfigsection, config, settings.Timeout);
            settings.RefreshEnable = GetConfigValue("refreshEnable", clientConfigsection, config, false);
            settings.RefreshInterval =
                GetConfigValue("refreshInterval", clientConfigsection, config, settings.RefreshInterval);
        }

        private static T GetConfigValue<T>(this string key, IConfiguration clientConfigsection,
            IConfiguration resolve, T def)
        {
            var value = clientConfigsection.GetValue<string>(key);
            if (!string.IsNullOrWhiteSpace(value))
                value = PropertyPlaceholderHelper.ResolvePlaceholders(value, resolve);
            return value.CastTo(def);
        }

        private static int GetRetryMaxAttempts(IConfiguration clientConfigsection, IConfiguration resolve, int def)
        {
            return GetConfigValue("retry:maxAttempts", clientConfigsection, resolve, def);
        }

        private static double GetRetryMultiplier(IConfiguration clientConfigsection, IConfiguration resolve, double def)
        {
            return GetConfigValue("retry:multiplier", clientConfigsection, resolve, def);
        }

        private static int GetRetryMaxInterval(IConfiguration clientConfigsection, IConfiguration resolve, int def)
        {
            return GetConfigValue("retry:maxInterval", clientConfigsection, resolve, def);
        }

        private static int GetRetryInitialInterval(IConfiguration clientConfigsection, IConfiguration resolve, int def)
        {
            return GetConfigValue("retry:initialInterval", clientConfigsection, resolve, def);
        }

        private static bool GetRetryEnabled(IConfiguration clientConfigsection, IConfiguration resolve, bool def)
        {
            return GetConfigValue("retry:enabled", clientConfigsection, resolve, def);
        }

        private static bool GetFailFast(IConfiguration clientConfigsection, IConfiguration resolve, bool def)
        {
            return GetConfigValue("failFast", clientConfigsection, resolve, def);
        }

        private static bool GetEnabled(IConfiguration clientConfigsection, IConfiguration resolve, bool def)
        {
            return GetConfigValue("enabled", clientConfigsection, resolve, def);
        }

        private static string GetToken(IConfiguration clientConfigsection, IConfiguration resolve)
        {
            return GetConfigValue<string>("token", clientConfigsection, resolve, null);
        }

        private static int GetTimeout(IConfiguration clientConfigsection, IConfiguration resolve, int def)
        {
            return GetConfigValue("timeout", clientConfigsection, resolve, def);
        }

        private static string GetUri(IConfiguration clientConfigsection, IConfiguration resolve, string def)
        {
            var uri = Environment.GetEnvironmentVariable("CONFIG_HOST");
            if (!string.IsNullOrWhiteSpace(uri) && uri.IsUrl())
                return uri;
            return GetConfigValue("uri", clientConfigsection, resolve, def);
        }

        private static string GetPassword(IConfiguration clientConfigsection, IConfiguration resolve)
        {
            return GetConfigValue<string>("password", clientConfigsection, resolve, null);
        }

        private static string GetUsername(IConfiguration clientConfigsection, IConfiguration resolve)
        {
            return GetConfigValue<string>("username", clientConfigsection, resolve, null);
        }

        private static string GetLabel(IConfiguration clientConfigsection, IConfiguration resolve)
        {
            return GetConfigValue<string>("label", clientConfigsection, resolve, null);
        }

        private static string GetApplicationName(IConfiguration primary, IConfiguration config, string defName)
        {
            return GetConfigValue("application", primary, config, defName);
        }

        private static bool GetCertificateValidation(IConfiguration clientConfigsection, IConfiguration resolve, bool def)
        {
            return GetConfigValue("validate_certificates", clientConfigsection, resolve, def);
        }
    }
}
