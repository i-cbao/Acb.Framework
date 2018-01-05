using Acb.Dapper.Adapters;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Acb.Dapper.Mysql
{
    public class MySqlConnectionAdapter : IDbConnectionAdapter
    {
        public string ProviderName => "Mysql";
        public Type ConnectionType => typeof(MySqlConnection);

        public string FormatSql(string sql)
        {
            return sql.Replace("[", "`").Replace("]", "`").Replace("\"", "`");
        }

        public IDbConnection Create()
        {
            return MySqlClientFactory.Instance.CreateConnection();
        }
    }
}
