using Acb.Core.Extensions;
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

        public string PageSql(string sql, string columns, string order)
        {
            var countSql = sql.Replace(columns, "COUNT(1) ");
            if (order.IsNotNullOrEmpty())
            {
                countSql = countSql.Replace($" {order}", string.Empty);
            }
            sql =
                $"{sql} LIMIT @pageSize OFFSET (@pageIndex-1) * @pageSize;{countSql};";
            return sql;
        }

        public IDbConnection Create()
        {
            return NpgsqlFactory.Instance.CreateConnection();
        }
    }
}
