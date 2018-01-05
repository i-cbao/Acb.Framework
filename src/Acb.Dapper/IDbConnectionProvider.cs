using System;
using System.Data;

namespace Acb.Dapper
{
    /// <summary> 数据库连接提供者接口 </summary>
    public interface IDbConnectionProvider
    {
        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <param name="threadCache">是否启用线程级缓存</param>
        IDbConnection Connection(string connectionName = null, bool threadCache = true);

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName">连接名称</param>
        /// <param name="threadCache">是否启用线程级缓存</param>
        IDbConnection Connection(Enum connectionName, bool threadCache = true);
    }
}
