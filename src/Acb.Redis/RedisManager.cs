using Acb.Core.Dependency;
using Acb.Core.Exceptions;
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
    public class RedisManager : ISingleDependency, IDisposable
    {
        private const string Prefix = "redis:";
        private const string DefaultConfigName = "redisDefault";
        private const string DefaultDbConfigName = "redisDefaultDb";
        private const string DefaultName = "default";
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections;
        public RedisManager()
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

        private static string GetConfigName(string configName)
        {
            var defaultName = DefaultConfigName.Config(DefaultName);
            return string.IsNullOrWhiteSpace(configName) ? defaultName : configName;
        }

        private static string GetConnectionString(string configName)
        {
            var connectionString = $"{Prefix}{configName}".Config(string.Empty);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new BusiException($"{Prefix}{configName}配置异常");
            return connectionString;
        }

        private ConnectionMultiplexer Connect(string connectionString)
        {
            var opts = ConfigurationOptions.Parse(connectionString);
            return Connect(opts);
        }

        private ConnectionMultiplexer Connect(ConfigurationOptions configOpts)
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

        private ConnectionMultiplexer GetConnection(string configName)
        {
            configName = GetConfigName(configName);
            var connectionString = GetConnectionString(configName);
            var opts = ConfigurationOptions.Parse(connectionString);
            return GetConnection(configName, opts);
        }

        private ConnectionMultiplexer GetConnection(string configName, ConfigurationOptions configOpts)
        {
            configName = GetConfigName(configName);
            var conn = _connections.GetOrAdd(configName, p => Connect(configOpts));
            if (conn != null && conn.IsConnected)
                return conn;
            conn?.Dispose();
            conn = Connect(configOpts);
            _connections[configName] = conn;
            return conn;
        }

        /// <summary> 获取Database </summary>
        /// <param name="configName"></param>
        /// <param name="defaultDb"></param>
        /// <returns></returns>
        public IDatabase GetDatabase(string configName = null, int defaultDb = -1)
        {
            if (defaultDb < 0)
            {
                defaultDb = DefaultDbConfigName.Config(-1);
            }

            var conn = GetConnection(configName);
            return conn.GetDatabase(defaultDb);
        }

        /// <summary> 获取Server </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="endPointsIndex"></param>
        /// <returns></returns>
        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            configName = GetConfigName(configName);

            var connectionString = GetConnectionString(configName);
            var confOption = ConfigurationOptions.Parse(connectionString);

            return GetConnection(configName, confOption).GetServer(confOption.EndPoints[endPointsIndex]);
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
