using Acb.Core.Extensions;
using Acb.Core.Helper;
using System.IO;

namespace Acb.Core.Config.Center
{
    internal static class ConfigCacheHelper
    {
        private const string CacheDirectory = "_config_cache";
        private const string EncodeKey = "#&#Ddv31";
        private const string EncodeIv = "%22de2^!";
        private static string CachePath(this string url)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), CacheDirectory);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Path.Combine(path, $"{url.Md5()}");
        }

        internal static void SetConfig(this string url, string content)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;
            var path = url.CachePath();
            var data = EncryptHelper.SymmetricEncrypt(content, EncryptHelper.SymmetricFormat.DES, EncodeKey, EncodeIv);
            File.WriteAllText(path, data);
        }

        internal static string GetConfig(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;
            var path = url.CachePath();
            if (!File.Exists(path))
                return string.Empty;
            var content = File.ReadAllText(path);
            return EncryptHelper.SymmetricDecrypt(content, EncryptHelper.SymmetricFormat.DES, EncodeKey, EncodeIv);
        }
    }
}
