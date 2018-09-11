using Acb.Core.Data.Config;
using Acb.Core.Logging;
using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using Acb.Core.Data.Adapters;

namespace Acb.Core.Data
{
    /// <summary> 数据库连接管理 </summary>
    public class DefaultDbConnectionProvider : IDbConnectionProvider
    {
        private readonly ILogger _logger;

        public DefaultDbConnectionProvider()
        {
            _logger = LogManager.Logger<DefaultDbConnectionProvider>();
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <returns></returns>
        public IDbConnection Connection(string connectionName = null)
        {
            var connectionConfig = ConnectionConfig.Config(connectionName);
            if (connectionConfig == null || string.IsNullOrWhiteSpace(connectionConfig.ConnectionString))
                throw new ArgumentException($"未找到connectionName为{connectionName}的数据库配置");
            return Connection(connectionConfig.ConnectionString, connectionConfig.ProviderName);
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <returns></returns>
        public IDbConnection Connection(Enum connectionName)
        {
            return Connection(connectionName.ToString());
        }

        public IDbConnection Connection(string connectionString, string providerName)
        {
            var adapter = DbConnectionManager.Create(providerName);
            var connection = adapter.Create();
            if (connection == null)
                throw new Exception("创建数据库连接失败");
            _logger.Debug($"Create Connection [{providerName}]:{connectionString}");
            connection.ConnectionString = connectionString;
            return connection;
        }

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
            return sb.ToString();
        }
    }
}
