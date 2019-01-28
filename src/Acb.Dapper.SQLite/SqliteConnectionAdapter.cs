using Acb.Core.Data;
using Acb.Core.Extensions;
using System;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Acb.Dapper.SQLite
{
    public class SqliteConnectionAdapter : IDbConnectionAdapter
    {
        public string ProviderName => "SQLite";
        public Type ConnectionType => typeof(SQLiteConnection);

        public string FormatSql(string sql)
        {
            return sql;
        }

        public string PageSql(string sql, string columns, string order)
        {
            var countSql = sql.Replace(columns, "COUNT(1) ");
            if (order.IsNotNullOrEmpty())
            {
                countSql = countSql.Replace($" {order}", string.Empty);
            }
            if (countSql.IsMatch("group by", RegexOptions.IgnoreCase))
                countSql = $"SELECT COUNT(1) FROM ({countSql}) AS count_t";
            sql =
                $"{sql} LIMIT @size OFFSET @skip;{countSql};";
            return sql;
        }

        public IDbConnection Create()
        {
            return new SQLiteConnection();
        }
    }
}
