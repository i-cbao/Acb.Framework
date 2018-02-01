using Acb.Core.Extensions;
using Acb.Dapper.Adapters;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Acb.Dapper.Mysql
{
    public class MySqlConnectionAdapter : IDbConnectionAdapter
    {
        public string ProviderName => "MySql";
        public Type ConnectionType => typeof(MySqlConnection);

        public string FormatSql(string sql)
        {
            return sql.Replace("[", "`").Replace("]", "`").Replace("\"", "`");
        }

        public string PageSql(string sql, string columns, string order)
        {
            var countSql = sql.Replace(columns, "COUNT(1) ");
            if (order.IsNotNullOrEmpty())
            {
                countSql = countSql.Replace($" {order}", string.Empty);
            }
            sql =
                $"{sql} LIMIT (@index-1) * @pageSize,@pageSize;{countSql};";
            return sql;
        }

        public IDbConnection Create()
        {
            return MySqlClientFactory.Instance.CreateConnection();
        }
    }
}
