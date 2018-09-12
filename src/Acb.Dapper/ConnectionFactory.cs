﻿using Acb.Core.Data;
using Acb.Core.Data.Config;
using Acb.Core.Dependency;
using Acb.Core.Helper;
using Acb.Core.Logging;
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
    public class ConnectionFactory : ISingleDependency
    {
        private readonly ConcurrentDictionary<Thread, Dictionary<string, ConnectionStruct>> ConnectionCache;
        private static readonly object LockObj = new object();
        private int _createCount;
        private int _removeCount;
        private int _cacheCount;
        private int _clearCount;
        private readonly Timer _clearTimer;
        private bool _clearTimerRun;
        private readonly ILogger _logger;

        public ConnectionFactory()
        {
            ConnectionCache = new ConcurrentDictionary<Thread, Dictionary<string, ConnectionStruct>>();
            _logger = LogManager.Logger<ConnectionFactory>();
            //配置文件改变时，清空缓存
            ConfigHelper.Instance.ConfigChanged += name =>
            {
                ConnectionCache.Clear();
            };
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
            if (ConnectionCache.Count == 0)
            {
                _clearTimerRun = false;
                _clearTimer.Stop();
            }
        }

        /// <summary> 清理失效的线程级缓存 </summary>
        private void ClearDict()
        {
            if (ConnectionCache.Count == 0)
                return;
            foreach (var key in ConnectionCache.Keys)
            {
                if (!ConnectionCache.TryGetValue(key, out var connDict))
                    continue;
                foreach (var name in connDict.Keys)
                {
                    if (key.IsAlive && connDict[name].IsAlive())
                        continue;
                    var conn = connDict[name];
                    if (connDict.Remove(name))
                    {
                        _removeCount++;
                        conn?.Dispose();
                    }
                }
                if (connDict.Count == 0)
                    ConnectionCache.TryRemove(key, out connDict);
            }
        }

        /// <summary> 创建数据库连接 </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IDbConnection Create(ConnectionConfig config)
        {

            //DbProviderFactories.GetFactory(connectionConfig.ProviderName).CreateConnection();
            _createCount++;
            var adapter = DbConnectionManager.Create(config.ProviderName);
            var connection = adapter.Create();
            if (connection == null)
                throw new Exception("创建数据库连接失败");
            connection.ConnectionString = config.ConnectionString;
            _logger.Debug($"Create Connection: {config.ConnectionString}");
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
                var config = ConnectionConfig.Config(connectionName);
                if (config == null || string.IsNullOrWhiteSpace(config.ConnectionString))
                    throw new ArgumentException($"未找到的数据库配置");
                var key = config.Name;
                if (!threadCache)
                    return Create(config);
                var connectionKey = Thread.CurrentThread;

                if (!ConnectionCache.TryGetValue(connectionKey, out var connDict))
                {
                    connDict = new Dictionary<string, ConnectionStruct>();
                    if (!ConnectionCache.TryAdd(connectionKey, connDict))
                    {
                        throw new Exception("Can not set db connection!");
                    }
                }

                if (connDict.ContainsKey(key))
                {
                    _cacheCount++;
                    return connDict[key].GetConnection();
                }

                connDict.Add(key, new ConnectionStruct(Create(config)));

                if (!_clearTimerRun)
                {
                    _clearTimer.Start();
                    _clearTimerRun = true;
                }

                return connDict[key].GetConnection();
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

        public IDbConnection Connection(string connectionString, string provider)
        {
            return Create(new ConnectionConfig { ProviderName = provider, ConnectionString = connectionString });
        }

        /// <summary> 缓存总数/// </summary>
        public int Count => ConnectionCache.Sum(t => t.Value.Count);

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
            foreach (var connectionStruct in ConnectionCache)
            {
                foreach (var item in connectionStruct.Value)
                {
                    sb.AppendLine(item.ToString());
                }
            }
            sb.AppendLine($"create:{_createCount},total:{Count},useCache:{_cacheCount},clear:{_clearCount},remove:{_removeCount}");
            return sb.ToString();
        }
    }
}
