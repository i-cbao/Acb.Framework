using Acb.Core;
using Acb.Core.Config;
using Acb.Core.Extensions;
using Acb.Dapper.Adapters;
using Acb.Dapper.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Acb.Dapper
{
    /// <summary> 数据库连接管理 </summary>
    public class ConnectionFactory : IDbConnectionProvider
    {
        /// <summary> 默认连接名 </summary>
        public string ConnectionName => "connectionName".Config(string.Empty);

        private static readonly ConcurrentDictionary<Thread, Dictionary<string, ConnectionStruct>> ConnectionDictionary;
        private static readonly object LockObj = new object();
        private int _removeCount;
        private int _cacheCount;
        private int _clearCount;
        private readonly Timer _clearTimer;
        private bool _clearTimerRun;

        static ConnectionFactory()
        {
            ConnectionDictionary = new ConcurrentDictionary<Thread, Dictionary<string, ConnectionStruct>>();
            //配置文件改变时，清空缓存
            ConfigManager.Instance.Change += name =>
            {
                if (name == ConfigUtils<DataBaseConfig>.Instance.FileName)
                {
                    ConnectionDictionary.Clear();
                }
            };
        }

        private ConnectionFactory()
        {
            _clearTimer = new Timer(1000 * 60);
            _clearTimer.Elapsed += ClearTimerElapsed;
            _clearTimer.Enabled = true;
            _clearTimer.Stop();
            _clearTimerRun = false;
        }

        private void ClearTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _clearCount++;
            ClearDict();
            if (ConnectionDictionary.Count == 0)
            {
                _clearTimerRun = false;
                _clearTimer.Stop();
            }
        }

        /// <summary> 单例 </summary>
        public static ConnectionFactory Instance
            =>
                Singleton<ConnectionFactory>.Instance ??
                (Singleton<ConnectionFactory>.Instance = new ConnectionFactory());


        /// <summary> 清理失效的线程级缓存 </summary>
        private void ClearDict()
        {
            if (ConnectionDictionary.Count == 0)
                return;
            foreach (var key in ConnectionDictionary.Keys)
            {
                if (!ConnectionDictionary.TryGetValue(key, out var connDict))
                    continue;
                foreach (var name in connDict.Keys)
                {
                    if (key.IsAlive && connDict[name].IsAlive())
                        continue;
                    if (connDict.Remove(name))
                    {
                        _removeCount++;
                        connDict[name]?.Dispose();
                    }
                }
                if (connDict.Count == 0)
                    ConnectionDictionary.TryRemove(key, out connDict);
            }
        }

        /// <summary> 创建数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        private static IDbConnection Create(string connectionName)
        {
            var config = ConfigUtils<DataBaseConfig>.Config;
            if (config == null)
                throw new ArgumentException("未找到database.config配置文件");
            var connectionConfig = config.Get(connectionName);
            if (connectionConfig == null || string.IsNullOrWhiteSpace(connectionConfig.ConnectionString))
                throw new ArgumentException($"未找到connectionName为{connectionName}的数据库配置");
            //DbProviderFactories.GetFactory(connectionConfig.ProviderName).CreateConnection();
            var adapter = DbConnectionManager.Create(connectionConfig.ProviderName);
            var connection = adapter.Create();
            if (connection == null)
                throw new Exception("创建数据库连接失败");
            connection.ConnectionString = connectionConfig.ConnectionString;
            return connection;
        }


        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <param name="threadCache">是否启用线程缓存</param>
        /// <returns></returns>
        public IDbConnection Connection(string connectionName = null, bool threadCache = true)
        {
            lock (LockObj)
            {
                if (string.IsNullOrWhiteSpace(connectionName))
                    connectionName = ConnectionName;
                if (string.IsNullOrWhiteSpace(connectionName))
                    throw new ArgumentException($"{nameof(connectionName)}不能为空");
                if (!threadCache)
                    return Create(connectionName);
                var connectionKey = Thread.CurrentThread;

                if (!ConnectionDictionary.TryGetValue(connectionKey, out var connDict))
                {
                    connDict = new Dictionary<string, ConnectionStruct>();
                    if (!ConnectionDictionary.TryAdd(connectionKey, connDict))
                    {
                        throw new Exception("Can not set db connection!");
                    }
                }
                if (connDict.ContainsKey(connectionName))
                {
                    _cacheCount++;
                    return connDict[connectionName].GetConnection();
                }

                connDict.Add(connectionName, new ConnectionStruct(Create(connectionName)));

                if (!_clearTimerRun)
                {
                    _clearTimer.Start();
                    _clearTimerRun = true;
                }
                return connDict[connectionName].GetConnection();
            }
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <param name="threadCache">是否启用线程缓存</param>
        /// <returns></returns>
        public IDbConnection Connection(Enum connectionName, bool threadCache = true)
        {
            return Connection(connectionName.ToString(), threadCache);
        }

        /// <summary> 缓存总数/// </summary>
        public int Count => ConnectionDictionary.Sum(t => t.Value.Count);

        /// <summary> 连接缓存信息 </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            var proc = Process.GetCurrentProcess();
            sb.AppendLine($"专用工作集内存：{proc.PrivateMemorySize64 / 1024.0}kb");
            sb.AppendLine($"工作集内存：{proc.WorkingSet64 / 1024.0}kb");
            sb.AppendLine($"最大内存：{proc.PeakWorkingSet64 / 1024.0}kb");
            sb.AppendLine($"线程数：{proc.Threads.Count}");
            foreach (var connectionStruct in ConnectionDictionary)
            {
                foreach (var item in connectionStruct.Value)
                {
                    sb.AppendLine(item.ToString());
                }
            }
            sb.AppendLine($"total:{Count},useCache:{_cacheCount},clear:{_clearCount},remove:{_removeCount}");
            return sb.ToString();
        }
    }
}
