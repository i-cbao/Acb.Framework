using Acb.Core.Domain.Dtos;
using Acb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Acb.Core.Logging
{
    /// <summary> 字段日志 </summary>
    public class FieldLog
    {
        /// <summary> 字段名 </summary>
        public string FieldName { get; set; }

        /// <summary> 字段描述 </summary>
        public string FieldDesc { get; set; }

        /// <summary> 变更前的值 </summary>
        public string BeforeValue { get; set; }

        /// <summary> 变更前的值 </summary>
        public string BeforeValueDesc { get; set; }

        /// <summary> 变更后的值 </summary>
        public string AfterValue { get; set; }

        /// <summary> 变更后的值 </summary>
        public string AfterValueDesc { get; set; }
    }

    /// <summary>
    /// 实体日志
    /// </summary>
    public static class MapLoggerHelper
    {
        private static string GetValue(this object value)
        {
            if (value == null) return null;
            var type = value.GetType().GetUnNullableType();
            if (type == typeof(DateTime))
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            if (type.IsEnum)
                return ((int)value).ToString();
            if (type == typeof(decimal))
                return ((decimal)value).ToString(CultureInfo.InvariantCulture);
            return value.ToString();
        }

        /// <summary> 带详细日志记录的字段赋值 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="valueFunc">值获取方法</param>
        /// <param name="descFunc">描述获取方法</param>
        /// <param name="isReset">是否重置，false:为默认值时不修改</param>
        /// <param name="isSetValue">是否更新源实体</param>
        /// <param name="fields">字段列表</param>
        /// <returns></returns>
        public static List<FieldLog> MapWithLogs<T>(this T model,
            Func<string, (bool, object)> valueFunc,
            Func<string, object, string> descFunc = null,
            bool isReset = false, bool isSetValue = true, params string[] fields) where T : class
        {
            var details = new List<FieldLog>();
            if (model == null || valueFunc == null)
                return details;
            var props = typeof(T).GetProperties();
            if (!fields.IsNullOrEmpty())
            {
                props = props.Where(t => fields.Contains(t.Name)).ToArray();
            }

            foreach (var prop in props)
            {
                if (!prop.CanRead || prop.GetCustomAttribute<KeyAttribute>() != null)
                    continue;
                if (isSetValue && !prop.CanWrite)
                    continue;
                var (exists, after) = valueFunc.Invoke(prop.Name);
                if (!exists) continue;
                //不重置时，判断默认值
                if (!isReset && (after == null || after.Equals(prop.PropertyType.DefaultValue())))
                    continue;
                var before = prop.GetValue(model);
                after = after.CastTo(prop.PropertyType);
                //不存在 or 相等 则返回
                if (Equals(before, after))
                    continue;
                var inputDto = new FieldLog
                {
                    FieldName = prop.Name,
                    FieldDesc = prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? prop.Name,
                    BeforeValue = before.GetValue(),
                    AfterValue = after.GetValue()
                };
                if (descFunc != null)
                {
                    inputDto.BeforeValueDesc = descFunc.Invoke(prop.Name, before);
                    inputDto.AfterValueDesc = descFunc.Invoke(prop.Name, after);
                }

                details.Add(inputDto);
                if (isSetValue && prop.CanWrite)
                    prop.SetValue(model, after);
            }

            return details;
        }

        /// <summary> 带详细日志记录的字段赋值 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dict"></param>
        /// <param name="descFunc">描述获取方法</param>
        /// <param name="isReset">是否重置，false:为默认值时不修改</param>
        /// <param name="isSetValue"></param>
        /// <returns></returns>
        public static List<FieldLog> MapWithLogs<T>(this T model,
            IDictionary<string, object> dict,
            Func<string, object, string> descFunc = null, bool isReset = false, bool isSetValue = true) where T : class
        {
            if (dict == null || !dict.Any())
                return new List<FieldLog>();
            return MapWithLogs(model,
                name => dict.ContainsKey(name) ? (true, dict.GetValue<object>(name)) : (false, null),
                descFunc, isReset, isSetValue, dict.Keys.ToArray());
        }

        /// <summary> 带详细日志记录的字段赋值 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dto"></param>
        /// <param name="descFunc">描述获取方法</param>
        /// <param name="isReset">是否重置，false:为默认值时不修改</param>
        /// <param name="isSetValue">是否更新源实体</param>
        /// <param name="fields">字段列表</param>
        /// <returns></returns>
        public static List<FieldLog> MapWithLogs<T>(this T model,
            DDto dto, Func<string, object, string> descFunc = null, bool isReset = false, bool isSetValue = true,
            params string[] fields)
            where T : class
        {
            if (dto == null) return new List<FieldLog>();
            var props = dto.GetType().GetProperties();
            return MapWithLogs(model,
                name =>
                {
                    var prop = props.FirstOrDefault(t => t.Name == name);
                    return prop == null || !prop.CanRead ? (false, null) : (true, prop.GetValue(dto));
                },
                descFunc, isReset, isSetValue, fields);
        }
    }
}
