using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using System.Collections.Concurrent;
using Acb.Core.Config;

namespace Acb.MongoDb
{
    public class MongoManager
    {
        private readonly ConcurrentDictionary<string, MongoHelper> _mongoCaches;

        private MongoManager()
        {
            _mongoCaches = new ConcurrentDictionary<string, MongoHelper>();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                _mongoCaches.Clear();
            };
        }

        /// <summary> 单例模式 </summary>
        public static MongoManager Instance => Singleton<MongoManager>.Instance ??
                                               (Singleton<MongoManager>.Instance = new MongoManager());

        public MongoHelper GetHelper(string database, string configName = null)
        {
            if (_mongoCaches.ContainsKey(database) && _mongoCaches.TryGetValue(database, out var helper))
                return helper;
            var config = MongoConfig.Config(configName);
            config.Database = database;
            helper = new MongoHelper(config);
            _mongoCaches.TryAdd(database, helper);
            return helper;
        }
    }
}
