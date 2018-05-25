using Acb.ConfigCenter.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace Acb.ConfigCenter
{
    public class ConfigManager
    {
        private readonly IConfigurationRoot _config;
        private readonly ConcurrentDictionary<string, object> _configeCache;
        private readonly string _configDirectory;
        public event Action<string> Change;
        private const string ConfigExtension = ".json";

        public ConfigManager()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            _config = builder.Build();
            _configeCache = new ConcurrentDictionary<string, object>();
            var dir = _config.GetValue<string>("configPath");
            _configDirectory = Path.Combine(Directory.GetCurrentDirectory(), dir);
            //文件监控
            var watcher = new FileSystemWatcher(_configDirectory)
            {
                IncludeSubdirectories = true,
                Filter = $"*{ConfigExtension}", //"*.config|*.xml"多个扩展名不受支持！
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size
            };
            watcher.Changed += Reset;
            watcher.Deleted += Reset;
            watcher.Renamed += Reset;
            watcher.Created += Reset;
            watcher.EnableRaisingEvents = true;
        }

        private void Reset(object sender, FileSystemEventArgs e)
        {
            if (_configeCache.ContainsKey(e.Name))
                _configeCache.TryRemove(e.Name, out var _);
            Change?.Invoke(e.Name);
        }

        private object ReadFile(string file)
        {
            var path = Path.Combine(_configDirectory, file);
            if (!File.Exists(path))
                return null;
            var content = File.ReadAllText(path, Encoding.UTF8);
            var ext = Path.GetExtension(file);
            switch (ext)
            {
                case ".yml":
                    var d = new Deserializer();
                    using (var reader = new StringReader(content))
                    {
                        return d.Deserialize(reader);
                    }
            }

            return JsonConvert.DeserializeObject(content);
        }

        private static long Timestamp(DateTime? time = null)
        {
            time = time ?? DateTime.Now;
            return (long)(time.Value.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary> 获取配置 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public object Get(string module, string env)
        {
            var file = $"{module}-{env}{ConfigExtension}";
            var config = _configeCache.GetOrAdd(file, ReadFile);
            if (config != null)
                return config;
            _configeCache.TryRemove(file, out var _);
            file = $"{module}{ConfigExtension}";
            config = _configeCache.GetOrAdd(file, ReadFile);
            if (config == null)
                _configeCache.TryRemove(file, out var _);
            return config;
        }

        /// <summary> 添加配置 </summary>
        /// <param name="file"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Save(string file, string config)
        {
            file = $"{file}{ConfigExtension}";
            var path = Path.Combine(_configDirectory, file);
            if (File.Exists(path))
            {
                _configeCache.TryRemove(file, out var _);
                File.Copy(path, path.Replace(ConfigExtension, $"_{Timestamp()}.bak"));
            }

            File.WriteAllText(path, config, Encoding.UTF8);
            return true;
        }

        /// <summary> 删除配置文件 </summary>
        /// <param name="file"></param>
        public void Remove(string file)
        {
            var name = string.Concat(file, ConfigExtension);
            var path = Path.Combine(_configDirectory, name);
            if (!File.Exists(path)) return;
            File.Move(path, path.Replace(ConfigExtension, $"_{Timestamp()}.bak"));
            _configeCache.TryRemove(name, out var _);
        }

        /// <summary> 配置文件列表 </summary>
        /// <returns></returns>
        public List<string> List()
        {
            var files = Directory.GetFiles(_configDirectory, $"*{ConfigExtension}");
            return files.Select(t => Path.GetFileNameWithoutExtension(t).Split('-')[0]).Distinct().OrderBy(t => t)
                .ToList();
        }

        public SecurityDto GetSecurity()
        {
            return _config.GetSection("security").Get<SecurityDto>();
        }
    }

    internal static class ConfigManagerMiddleware
    {
        public static void AddConfigManager(this IServiceCollection service)
        {
            service.AddSingleton(new ConfigManager());
        }
    }
}
