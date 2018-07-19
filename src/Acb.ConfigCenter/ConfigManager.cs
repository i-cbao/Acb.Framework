using Acb.ConfigCenter.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        private const string ConfigDir = "_config";
        private static readonly string[] Modes = { "dev", "test", "ready", "prod" };
        private readonly ConcurrentDictionary<string, long> _configVersions;

        public ConfigManager()
        {
            _configVersions = new ConcurrentDictionary<string, long>();
            _configeCache = new ConcurrentDictionary<string, object>();
            _configDirectory = Path.Combine(Directory.GetCurrentDirectory(), ConfigDir);

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(Path.Combine(_configDirectory, "application.json"), false, true);
            _config = builder.Build();
            _configExtension = _config.GetValue("configExt", ".json");

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

        public long Version(string module, string env)
        {
            var key = $"{module}_{env}";
            if (_configVersions.ContainsKey(key) && _configVersions.TryGetValue(key, out var version))
                return version;
            var file = Path.Combine(_configDirectory, $"{module}-{env}{_configExtension}");
            if (!File.Exists(file))
            {
                file = Path.Combine(_configDirectory, $"{module}{_configExtension}");
            }

            if (!File.Exists(file))
                version = 0;
            else
            {
                var info = new FileInfo(Path.Combine(_configDirectory, file));
                version = info.LastWriteTime.Timestamp();
            }

            _configVersions.TryAdd(key, version);
            return version;
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
            _configVersions.Clear();
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
            _configeCache.TryRemove(name, out _);
            _configVersions.Clear();
        }

        /// <summary> 配置文件列表 </summary>
        /// <returns></returns>
        public List<string> List()
        {
            var files = Directory.GetFiles(_configDirectory, $"*{_configExtension}");
            return files.Select(t =>
            {
                var file = Path.GetFileNameWithoutExtension(t);
                var arr = file.Split('-');
                if (arr.Length <= 1)
                    return file;
                var mode = arr[arr.Length - 1];
                if (Modes.Contains(mode))
                    return file.Substring(0, file.LastIndexOf('-'));
                return file;
            }).Distinct().OrderBy(t => t).ToList();
        }

        internal SecurityDto GetSecurity()
        {
            return _config.GetSection("security").Get<SecurityDto>();
        }
    }

    internal static class ConfigManagerMiddleware
    {
        public static void AddConfigManager(this IServiceCollection service)
        {
            service.TryAddSingleton(new ConfigManager());
        }
    }
}
