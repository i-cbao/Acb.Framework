using Acb.Core.Exceptions;
using Acb.Core.Logging;
using System.Collections.Concurrent;
using System.Data;

namespace Acb.Dapper.Adapters
{
    public static class DbConnectionManager
    {
        private static readonly ConcurrentDictionary<string, IDbConnectionAdapter> Adapters;
        private static ILogger Logger;

        static DbConnectionManager()
        {
            Adapters = new ConcurrentDictionary<string, IDbConnectionAdapter>();
            AddAdapter(new SqlServerAdapter());
            Logger = LogManager.Logger(nameof(DbConnectionManager));
        }

        public static void AddAdapter(IDbConnectionAdapter adapter)
        {
            if (Adapters.ContainsKey(adapter.ProviderName))
                return;
            Adapters.TryAdd(adapter.ProviderName, adapter);
        }

        public static IDbConnectionAdapter Create(string providerName = null)
        {
            providerName = string.IsNullOrWhiteSpace(providerName) ? SqlServerAdapter.Name : providerName;
            if (Adapters.TryGetValue(providerName, out var adapter))
                return adapter;
            throw new BusiException($"不支持的DbProvider：{providerName}");
        }

        public static string FormatSql(this IDbConnection conn, string sql)
        {
            foreach (var adapter in Adapters.Values)
            {
                if (adapter.ConnectionType == conn.GetType())
                {
                    sql = adapter.FormatSql(sql);
                    Logger.Debug(sql);
                    return sql;
                }
            }
            return sql;
        }

        public static string PagedSql(this IDbConnection conn, string sql, string columns, string order)
        {
            foreach (var adapter in Adapters.Values)
            {
                if (adapter.ConnectionType == conn.GetType())
                    return adapter.FormatSql(adapter.PageSql(sql, columns, order));
            }

            return sql;
        }
    }
}
