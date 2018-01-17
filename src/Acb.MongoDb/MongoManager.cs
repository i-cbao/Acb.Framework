using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using System.Collections.Concurrent;

namespace Acb.MongoDb
{
    public class MongoManager
    {
        private readonly ConcurrentDictionary<string, MongoHelper> _mongoCaches;
        private const string Prefix = "mongo:";
        private const string DefaultConfigName = "mongoDefault";
        private const string DefaultName = "default";

        private MongoManager()
        {
            _mongoCaches = new ConcurrentDictionary<string, MongoHelper>();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                _mongoCaches.Clear();
            };
        }

        public static MongoManager Instance => Singleton<MongoManager>.Instance ??
                                               (Singleton<MongoManager>.Instance = new MongoManager());

        public MongoHelper GetHelper(string database, string configName = null)
        {
            if (_mongoCaches.ContainsKey(database) && _mongoCaches.TryGetValue(database, out var helper))
                return helper;
            configName = string.IsNullOrWhiteSpace(configName) ? DefaultConfigName.Config(DefaultName) : configName;
            var config = $"{Prefix}{configName}".Config<MongoConfig>();
            helper = new MongoHelper(config, database);
            _mongoCaches.TryAdd(database, helper);
            return helper;
        }
    }
}
