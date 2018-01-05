using Acb.Core.Exceptions;
using System.Collections.Concurrent;
using System.Data;

namespace Acb.Dapper.Adapters
{
    public static class DbConnectionManager
    {
        private static readonly ConcurrentDictionary<string, IDbConnectionAdapter> Adapters;

        static DbConnectionManager()
        {
            Adapters = new ConcurrentDictionary<string, IDbConnectionAdapter>();
            AddAdapter(new SqlServerAdapter());
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
                    return adapter.FormatSql(sql);
            }
            return sql;
        }
    }
}
