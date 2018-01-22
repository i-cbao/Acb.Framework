using Acb.Core.Cache;
using Acb.Core.Extensions;
using Acb.Dapper.Adapters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acb.Dapper
{
    /// <summary> Dapper自定义扩展 </summary>
    public static class DapperExtension
    {
        private static readonly ICache DapperCache = CacheManager.GetCacher(nameof(DapperExtension));

        /// <summary> 查询到DataSet </summary>
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="formatVariable"></param>
        /// <param name="param"></param>
        /// <param name="adapter"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet QueryDataSet(this IDbConnection cnn, string sql, Func<string, string> formatVariable,
            object param = null, IDbDataAdapter adapter = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            var ds = new DataSet();
            var wasClosed = cnn.State == ConnectionState.Closed;
            if (wasClosed) cnn.Open();
            var command = cnn.CreateCommand();
            if (commandType.HasValue)
                command.CommandType = commandType.Value;
            if (commandTimeout.HasValue)
                command.CommandTimeout = commandTimeout.Value;
            command.CommandText = sql;
            if (param != null)
            {
                var ps = param.GetType().GetProperties();
                foreach (var propertyInfo in ps)
                {
                    var propType = propertyInfo.PropertyType;
                    var value = propertyInfo.GetValue(param);
                    if (propType.IsNullableType() && value == null)
                        continue;
                    var p = command.CreateParameter();
                    p.ParameterName = formatVariable(propertyInfo.Name);
                    p.Value = value;
                    command.Parameters.Add(p);
                }
            }
            adapter = adapter ?? new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(ds);
            if (wasClosed) cnn.Close();
            return ds;
        }

        /// <summary> 字段列表 </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static string[] Fields(this Type modelType)
        {
            var key = $"fields_{modelType.FullName}";
            var fields = DapperCache.Get<string[]>(key);
            if (fields != null && fields.Any()) return fields;
            var props = modelType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            fields = props.Select(t => t.PropName()).ToArray();
            DapperCache.Set(key, fields, TimeSpan.FromHours(2));
            return fields;
        }
        /// <summary> 生成insert语句 </summary>
        /// <returns></returns>
        public static string InsertSql(this Type modelType, string[] excepts = null)
        {
            var tableName = modelType.PropName();
            var key = $"insert_{modelType.FullName}";
            var sql = DapperCache.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(sql))
                return sql;
            var sb = new StringBuilder();
            sb.Append($"INSERT INTO [{tableName}]");

            var fields = Fields(modelType);
            if (excepts != null && excepts.Any())
                fields = fields.Except(excepts).ToArray();
            var fieldSql = string.Join(",", fields.Select(t => $"[{t}]"));
            var paramSql = string.Join(",", fields.Select(t => $"@{t}"));
            sb.Append($" ({fieldSql}) VALUES ({paramSql})");
            DapperCache.Set(key, sql, TimeSpan.FromHours(1));
            return sql;
        }

        /// <summary> 设置对象属性 </summary>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public static void SetPropValue(this object model, string propName, object value)
        {
            var type = model.GetType();
            var prop = type.GetProperty(propName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null)
                prop.SetValue(model, value);
        }

        /// <summary> 获取对象属性 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static T PropValue<T>(this object model, string propName)
        {
            var type = model.GetType();
            var prop = type.GetProperty(propName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return prop == null ? default(T) : prop.GetValue(model).CastTo<T>();
        }

        /// <summary> 获取对象属性 </summary>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string PropValue(this object model, string propName)
        {
            return PropValue<string>(model, propName);
        }

        /// <summary> 查询所有数据 </summary>
        public static IEnumerable<T> QueryAll<T>(this IDbConnection conn)
        {
            var type = typeof(T);
            var fields = type.Fields();
            var tableName = type.PropName();
            var columns = string.Join(",", fields.Select(t => $"[{t}]"));
            var sql = $"SELECT {columns} FROM [{tableName}]";
            sql = conn.FormatSql(sql);
            return conn.Query<T>(sql);
        }

        /// <summary> 根据主键查询单条 </summary>
        /// <param name="conn"></param>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public static T QueryById<T>(this IDbConnection conn, object key, string keyColumn = "id")
        {
            var type = typeof(T);
            var fields = type.Fields();
            var tableName = type.PropName();
            var columns = string.Join(",", fields.Select(t => $"[{t}]"));
            var sql = $"SELECT {columns} FROM [{tableName}] WHERE [{keyColumn}]=@id";
            sql = conn.FormatSql(sql);
            return conn.QueryFirstOrDefault<T>(sql, new { id = key });
        }

        /// <summary> 插入单条数据,不支持有自增列 </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="excepts">过滤项(如：自增ID)</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Insert<T>(this IDbConnection conn, T model, string[] excepts = null, IDbTransaction trans = null)
        {
            var type = typeof(T);
            var sql = type.InsertSql(excepts);
            sql = conn.FormatSql(sql);
            return conn.Execute(sql, model, trans);
        }

        /// <summary> 批量插入 </summary>
        /// <param name="conn"></param>
        /// <param name="models"></param>
        /// <param name="excepts"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int InsertBatch<T>(this IDbConnection conn, IEnumerable<T> models, string[] excepts = null, IDbTransaction trans = null)
        {
            var type = typeof(T);
            var sql = type.InsertSql(excepts);
            sql = conn.FormatSql(sql);
            return conn.Execute(sql, models.ToArray(), trans);
        }

        //public static int Update<T>(this IDbConnection conn, Expression<Func<T, dynamic>> propExpression, string where,
        //    object param = null, IDbTransaction trans = null)
        //{
        //    var tableName = typeof(T).PropName();
        //    SQL sql = $"UPDATE FROM [{tableName}] SET ";
        //    ReadOnlyCollection<MemberInfo> memberInfos = ((dynamic)propExpression.Body).Members;
        //    if (memberInfos.Count == 0) return 0;
        //    var ps = new DynamicParameters();
        //    foreach (var info in memberInfos)
        //    {
        //        var name = info.PropName();
        //        sql += $"[{name}]=@{name}";
        //        //ps.Add(name,propExpression);
        //        // :todo 
        //    }

        //    if (param != null)
        //        ps.AddDynamicParams(param);
        //    return conn.Execute(sql.ToString(), ps, trans);
        //}

        /// <summary> 删除数据 </summary>
        /// <param name="conn">连接</param>
        /// <param name="value">列值</param>
        /// <param name="keyColumn">列名</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public static int Delete<T>(this IDbConnection conn, object value, string keyColumn = "id", IDbTransaction trans = null)
        {
            var tableName = typeof(T).PropName();
            var sql = $"DELETE FROM [{tableName}] WHERE [{keyColumn}]=@value";
            sql = conn.FormatSql(sql);
            return conn.Execute(sql, new { value }, trans);
        }

        /// <summary> 删除 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int DeleteWhere<T>(this IDbConnection conn, string where, object param = null, IDbTransaction trans = null)
        {
            var tableName = typeof(T).PropName();
            var sql = $"DELETE FROM [{tableName}] WHERE {where}";
            sql = conn.FormatSql(sql);
            return conn.Execute(sql, param, trans);
        }

        /// <summary> 是否存在 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Exists<T>(this IDbConnection conn, string column, object value)
        {
            var tableName = typeof(T).PropName();
            var sql = $"SELECT COUNT(1) FROM [{tableName}] WHERE [{column}]=@value";
            sql = conn.FormatSql(sql);
            return conn.QueryFirstOrDefault<int>(sql, new { value }) > 0;
        }

        /// <summary> 是否存在 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExistsWhere<T>(this IDbConnection conn, string where = null, object param = null)
        {
            var tableName = typeof(T).PropName();
            SQL sql = $"SELECT COUNT(1) FROM [{tableName}]";
            if (!string.IsNullOrWhiteSpace(where))
                sql += $"WHERE {where}";
            var sqlStr = conn.FormatSql(sql.ToString());
            return conn.QueryFirstOrDefault<int>(sqlStr, param) > 0;
        }

        /// <summary> 最小 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long Min<T>(this IDbConnection conn, string column, string where = null, object param = null)
        {
            var tableName = typeof(T).PropName();
            SQL sql = $"SELECT MIN([{column}]) FROM [{tableName}]";
            if (!string.IsNullOrWhiteSpace(where))
                sql += $"WHERE {where}";
            var sqlStr = conn.FormatSql(sql.ToString());
            return conn.QueryFirstOrDefault<long>(sqlStr, param);
        }

        /// <summary> 最大 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long Max<T>(this IDbConnection conn, string column, string where = null, object param = null)
        {
            var tableName = typeof(T).PropName();
            SQL sql = $"SELECT MAX([{column}]) FROM [{tableName}]";
            if (!string.IsNullOrWhiteSpace(where))
                sql += $"WHERE {where}";
            var sqlStr = conn.FormatSql(sql.ToString());
            return conn.QueryFirstOrDefault<long>(sqlStr);
        }

        /// <summary> 自增数据 </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <param name="count"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Increment<T>(this IDbConnection conn, string column, object key, string keyColumn = "id",
            int count = 1, IDbTransaction trans = null)
        {
            var tableName = typeof(T).PropName();
            var sql = $"UPDATE [{tableName}] SET [{column}]=[{column}] + @count WHERE [{keyColumn}]=@id";
            sql = conn.FormatSql(sql);
            return conn.Execute(sql, new { id = key, count }, trans);
        }
    }
}
