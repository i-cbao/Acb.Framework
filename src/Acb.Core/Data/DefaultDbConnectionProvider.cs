using System;
using System.Data;

namespace Acb.Core.Data
{
    /// <summary> 数据库连接管理 </summary>
    public class DefaultDbConnectionProvider : IDbConnectionProvider
    {
        public ConnectionFactory Factory { private get; set; }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <returns></returns>
        public IDbConnection Connection(string connectionName = null)
        {
            return Factory.Connection(connectionName, false);
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <returns></returns>
        public IDbConnection Connection(Enum connectionName)
        {
            return Factory.Connection(connectionName, false);
        }

        public IDbConnection Connection(string connectionString, string providerName)
        {
            return Factory.Connection(connectionString, providerName, false);
        }
    }
}
