using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Dapper;
using System.Collections.Concurrent;
using System.Data;

namespace Acb.WebApi.Test.Connections
{
    public class ConnectionStruct : IConnectionStruct
    {
        private readonly ConcurrentDictionary<string, IDbConnection> _connCache;
        private readonly ILogger _logger;

        public ConnectionStruct()
        {
            _logger = LogManager.Logger<ConnectionStruct>();
            _logger.Info("connection create");
            _connCache = new ConcurrentDictionary<string, IDbConnection>();
        }

        public void Dispose()
        {
            _logger.Info("connection dispose");
            foreach (var conn in _connCache.Values)
            {
                conn.Dispose();
            }
        }

        public IDbConnection Connection(string name = null)
        {
            var connName = string.IsNullOrWhiteSpace(name) ? "dapperDefault".Config("default") : name;
            return _connCache.GetOrAdd(connName, key =>
            {
                var conn = ConnectionFactory.Instance.Connection(key, false);
                return conn;
            });
        }
    }
}
