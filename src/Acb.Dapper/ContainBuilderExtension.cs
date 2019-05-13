﻿using Acb.Core.Data.Config;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Acb.Dapper
{
    public static class ContainBuilderExtension
    {
        public static IServiceCollection AddDapper(this IServiceCollection services,
            Action<ConnectionConfig> configAction = null)
        {
            services.TryAddScoped<IUnitOfWork>(provider =>
            {
                var config = ConnectionConfig.Config();
                configAction?.Invoke(config);
                return new UnitOfWork(config.ConnectionString, config.ProviderName);
            });
            return services;
        }

        public static IServiceCollection AddDapper(this IServiceCollection services, string configName)
        {
            services.TryAddScoped<IUnitOfWork>(provider =>
            {
                var config = ConnectionConfig.Config(configName);
                return new UnitOfWork(config.ConnectionString, config.ProviderName);
            });
            return services;
        }

        /// <summary> select语句构建 </summary>
        /// <param name="type"></param>
        /// <param name="where">where</param>
        /// <param name="orderby">order by</param>
        /// <param name="excepts">排除字段</param>
        /// <param name="includes">包含字段</param>
        /// <param name="tableAlias">表别名</param>
        /// <returns></returns>
        public static string Select(this Type type, string where, string orderby = null, string[] excepts = null,
            string[] includes = null, string tableAlias = null)
        {
            var builder = new SqlBuilder();
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = where.Trim();
                if (where.StartsWith("where ", StringComparison.CurrentCultureIgnoreCase))
                    where = where.Substring(6);
                builder.Where(where);
            }

            if (!string.IsNullOrWhiteSpace(orderby))
            {
                orderby = orderby.Trim();
                if (orderby.StartsWith("order by ", StringComparison.CurrentCultureIgnoreCase))
                    orderby = orderby.Substring(9);
                builder.OrderBy(orderby);
            }

            var sql = builder.AddTemplate(
                $"SELECT {type.Columns(excepts, includes, tableAlias)} FROM [{type.PropName()}] /**where**/ /**orderby**/");
            return sql.RawSql;
        }
    }
}
