using StackExchange.Redis;

namespace Acb.EventBus.Redis
{
    public class RedisConnection : IRedisConnection
    {
        private readonly ConfigurationOptions _options;
        public RedisConnection(string connectionString)
        {
            _options = ConfigurationOptions.Parse(connectionString);
        }

        public RedisConnection(ConfigurationOptions opts)
        {
            _options = opts;
        }

        private ConnectionMultiplexer Connect()
        {
            //var points = string.Join<EndPoint>(",", configOpts.EndPoints.ToArray());
            //_logger.Info($"Create Redis: {points}");
            var conn = ConnectionMultiplexer.Connect(_options);
            //conn.ConfigurationChanged += (sender, e) =>
            //{
            //    _logger.Debug($"Redis Configuration changed: {e.EndPoint}");
            //};
            //conn.ConnectionRestored += (sender, e) => { _logger.Debug($"Redis ConnectionRestored: {e.EndPoint}"); };
            //conn.ErrorMessage += (sender, e) => { _logger.Error($"Redis Error{e.EndPoint}: {e.Message}"); };
            //conn.ConnectionFailed += (sender, e) =>
            //{
            //    _logger.Warn(
            //        $"Redis 重新连接：Endpoint failed: ${e.EndPoint}, ${e.FailureType},${e.Exception?.Message}");
            //};
            //conn.InternalError += (sender, e) => { _logger.Warn($"Redis InternalError:{e.Exception.Message}"); };
            return conn;
        }

        public ISubscriber GetSubscriber()
        {
            return Connect().GetSubscriber();
        }
    }
}
