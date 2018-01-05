using System;
using System.Data;
using System.Data.SqlClient;

namespace Acb.Dapper.Adapters
{
    public class SqlServerAdapter : IDbConnectionAdapter
    {
        public const string Name = "SqlServer";

        public string ProviderName => Name;
        public Type ConnectionType => typeof(SqlConnection);

        public string FormatSql(string sql)
        {
            return sql;
        }

        public IDbConnection Create()
        {
            return SqlClientFactory.Instance.CreateConnection();
        }
    }
}
