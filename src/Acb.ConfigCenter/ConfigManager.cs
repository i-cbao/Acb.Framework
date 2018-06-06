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
        private static string _configExtension;

        public ConfigManager()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            _config = builder.Build();
            _configeCache = new ConcurrentDictionary<string, object>();
            var dir = _config.GetValue<string>("configPath");
            _configExtension = _config.GetValue("configExt", ".json");
            _configDirectory = Path.Combine(Directory.GetCurrentDirectory(), dir);
            //文件监控
            var watcher = new FileSystemWatcher(_configDirectory)
            {
                IncludeSubdirectories = true,
                Filter = $"*{_configExtension}", //"*.config|*.xml"多个扩展名不受支持！
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
                _configeCache.TryRemove(e.Name, out _);
            Change?.Invoke(e.Name);
        }

        private static object ReadFile(string path)
        {
            if (!File.Exists(path))
                return null;
            var content = File.ReadAllText(path, Encoding.UTF8);
            switch (_configExtension)
            {
                case ".yml":
                    var d = new Deserializer();
                    using (var reader = new StringReader(content))
                    {
                        return d.Deserialize(reader);
                    }
                default:
                    return JsonConvert.DeserializeObject(content);
            }
        }

        private static void SaveFile(string path, string config)
        {
            switch (_configExtension)
            {
                case ".yml":
                    var obj = JsonConvert.DeserializeObject<dynamic>(config);
                    var builder = new SerializerBuilder();
                    builder.JsonCompatible();
                    var d = builder.Build();
                    using (var writer = new StreamWriter(path, false))
                    {
                        d.Serialize(writer, obj);
                    }
                    break;
                default:
                    File.WriteAllText(path, config, Encoding.UTF8);
                    break;
            }
        }

        /// <summary> 获取配置 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public object Get(string module, string env)
        {
            var file = $"{module}-{env}{_configExtension}";
            var config = _configeCache.GetOrAdd(file, t =>
            {
                var path = Path.Combine(_configDirectory, t);
                return ReadFile(path);
            });
            if (config != null)
                return config;
            _configeCache.TryRemove(file, out _);
            file = $"{module}{_configExtension}";
            config = _configeCache.GetOrAdd(file, t =>
            {
                var path = Path.Combine(_configDirectory, t);
                return ReadFile(path);
            });
            if (config == null)
                _configeCache.TryRemove(file, out _);
            return config;
        }

        /// <summary> 添加配置 </summary>
        /// <param name="file"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool Save(string file, string config)
        {
            file = $"{file}{_configExtension}";
            var path = Path.Combine(_configDirectory, file);
            if (File.Exists(path))
            {
                _configeCache.TryRemove(file, out _);
                File.Copy(path, path.Replace(_configExtension, $"_{DateTime.Now.Timestamp()}.bak"));
            }
            SaveFile(path, config);
            return true;
        }

        /// <summary> 删除配置文件 </summary>
        /// <param name="file"></param>
        public void Remove(string file)
        {
            var name = string.Concat(file, _configExtension);
            var path = Path.Combine(_configDirectory, name);
            if (!File.Exists(path)) return;
            File.Move(path, path.Replace(_configExtension, $"_{DateTime.Now.Timestamp()}.bak"));
            _configeCache.TryRemove(name, out var _);
        }

        /// <summary> 配置文件列表 </summary>
        /// <returns></returns>
        public List<string> List()
        {
            var files = Directory.GetFiles(_configDirectory, $"*{_configExtension}");
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
