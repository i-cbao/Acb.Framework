using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using System.Collections.Concurrent;

namespace Acb.MongoDb
{
    public class MongoManager : ISingleDependency
    {
        private readonly ConcurrentDictionary<string, MongoHelper> _mongoCaches;
        private const string Prefix = "mongo:";
        private const string DefaultConfigName = "mongoDefault";
        private const string DefaultName = "default";

        public MongoManager()
        {
            _mongoCaches = new ConcurrentDictionary<string, MongoHelper>();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                _mongoCaches.Clear();
            };
        }

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
