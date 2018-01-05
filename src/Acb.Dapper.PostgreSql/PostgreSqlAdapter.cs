using Acb.Dapper.Adapters;
using Npgsql;
using System;
using System.Data;

namespace Acb.Dapper.PostgreSql
{
    public class PostgreSqlAdapter : IDbConnectionAdapter
    {
        public string ProviderName => "PostgreSql";
        public Type ConnectionType => typeof(NpgsqlConnection);
        public string FormatSql(string sql)
        {
            return sql.Replace("@", ":").Replace("?", ":").Replace("[", "\"").Replace("]", "\"");
        }

        public IDbConnection Create()
        {
            return NpgsqlFactory.Instance.CreateConnection();
        }
    }
}
