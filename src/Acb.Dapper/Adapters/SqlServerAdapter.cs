using Acb.Core.Exceptions;
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

        public string PageSql(string sql, string columns, string order)
        {
            if (string.IsNullOrWhiteSpace(order))
            {
                throw new DException("need order by!");
            }

            var countSql = sql.Replace(columns, "COUNT(1) ").Replace($" {order}", string.Empty);

            sql = sql.Replace($" {order}", string.Empty);
            sql = sql.Replace(columns, $"ROW_NUMBER() OVER({order}) AS [paged_row],{columns}");

            sql =
                $"SELECT * FROM ({sql}) [_proj] WHERE [paged_row] BETWEEN ((@pageIndex-1)*@pageSize +1) AND (@pageIndex * @pageSize);{countSql};";
            return sql;
        }

        public IDbConnection Create()
        {
            return SqlClientFactory.Instance.CreateConnection();
        }
    }
}
