using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace Acb.Redis
{
    /// <summary> Redis管理器 </summary>
    public class RedisManager : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

        private RedisManager()
        {
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
            _logger = LogManager.Logger<RedisManager>();

            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                if (_connections.Count <= 0)
                    return;
                _connections.Values.Foreach(t => t.Close());
                _connections.Clear();
            };
        }

        /// <summary> 单例模式 </summary>
        public static RedisManager Instance => Singleton<RedisManager>.Instance ??
                                               (Singleton<RedisManager>.Instance = new RedisManager());

        private static RedisConfig GetConfig(string configName)
        {
            var config = RedisConfig.Config(configName);
            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new ArgumentException($"Redis:{config.Name}配置异常", nameof(configName));
            return config;
        }

        public ConnectionMultiplexer Connect(string connectionString)
        {
            var opts = ConfigurationOptions.Parse(connectionString);
            return Connect(opts);
        }

        public ConnectionMultiplexer Connect(ConfigurationOptions configOpts)
        {
            var points = string.Join<EndPoint>(",", configOpts.EndPoints.ToArray());
            _logger.Info($"Create Redis: {points}");
            var conn = ConnectionMultiplexer.Connect(configOpts);
            conn.ConfigurationChanged += (sender, e) =>
            {
                _logger.Debug($"Redis Configuration changed: {e.EndPoint}");
            };
            conn.ConnectionRestored += (sender, e) => { _logger.Debug($"Redis ConnectionRestored: {e.EndPoint}"); };
            conn.ErrorMessage += (sender, e) => { _logger.Error($"Redis Error{e.EndPoint}: {e.Message}"); };
            conn.ConnectionFailed += (sender, e) =>
            {
                _logger.Warn(
                    $"Redis 重新连接：Endpoint failed: ${e.EndPoint}, ${e.FailureType},${e.Exception?.Message}");
            };
            conn.InternalError += (sender, e) => { _logger.Warn($"Redis InternalError:{e.Exception.Message}"); };
            return conn;
        }

        public ConnectionMultiplexer GetConnection(string configName)
        {
            var config = GetConfig(configName);
            var opts = ConfigurationOptions.Parse(config.ConnectionString);
            return GetConnection(config.Name, opts);
        }

        private ConnectionMultiplexer GetConnection(string configName, ConfigurationOptions configOpts)
        {
            var conn = _connections.GetOrAdd(configName, p => Connect(configOpts));
            if (conn != null && conn.IsConnected)
                return conn;
            conn?.Dispose();
            conn = Connect(configOpts);
            _connections[configName] = conn;
            return conn;
        }

        public IDatabase GetDatabase(RedisConfig config, int defaultDb = -1)
        {
            if (string.IsNullOrWhiteSpace(config.Name))
                return Connect(config.ConnectionString).GetDatabase();
            return GetConnection(config.Name, ConfigurationOptions.Parse(config.ConnectionString))
                .GetDatabase(defaultDb);
        }

        /// <summary> 获取Database </summary>
        /// <param name="configName"></param>
        /// <param name="defaultDb"></param>
        /// <returns></returns>
        public IDatabase GetDatabase(string configName = null, int defaultDb = -1)
        {
            var conn = GetConnection(configName);
            return conn.GetDatabase(defaultDb);
        }

        /// <summary> 获取Server </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="endPointsIndex"></param>
        /// <returns></returns>
        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            var config = GetConfig(configName);
            var confOption = ConfigurationOptions.Parse(config.ConnectionString);
            return GetConnection(config.Name, confOption).GetServer(confOption.EndPoints[endPointsIndex]);
        }

        /// <summary> 获取Server </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="host">主机名</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public IServer GetServer(string configName, string host, int port)
        {
            var conn = GetConnection(configName);
            return conn.GetServer(host, port);
        }

        /// <summary> 获取订阅 </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public ISubscriber GetSubscriber(string configName = null)
        {
            return GetConnection(configName).GetSubscriber();
        }

        /// <summary> 释放资源 </summary>
        public void Dispose()
        {
            if (_connections == null || _connections.Count == 0)
                return;
            _connections.Values.Foreach(t => t.Close());
        }
    }
}
