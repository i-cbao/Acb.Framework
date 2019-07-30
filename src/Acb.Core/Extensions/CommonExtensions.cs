using Acb.Core.Serialize;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Acb.Core.Extensions
{
    /// <summary> 对象扩展辅助 </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// 对象转换为泛型
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CastTo<T>(this object obj)
        {
            return obj.CastTo(default(T));
        }

        /// <summary>
        /// 对象转换为泛型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="def"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CastTo<T>(this object obj, T def)
        {
            var value = obj.CastTo(typeof(T));
            if (value == null)
                return def;
            return (T)value;
        }

        /// <summary> 把对象类型转换为指定类型 </summary>
        /// <param name="obj"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object obj, Type conversionType)
        {
            if (obj == null)
            {
                return conversionType.IsGenericType ? Activator.CreateInstance(conversionType) : null;
            }
            if (conversionType.IsNullableType())
                conversionType = conversionType.GetUnNullableType();
            try
            {
                if (conversionType == obj.GetType())
                    return obj;
                if (conversionType.IsEnum)
                {
                    return obj is string s
                        ? Enum.Parse(conversionType, s)
                        : Enum.ToObject(conversionType, obj);
                }

                if (!conversionType.IsInterface && conversionType.IsGenericType)
                {
                    var innerType = conversionType.GetGenericArguments()[0];
                    var innerValue = CastTo(obj, innerType);
                    return Activator.CreateInstance(conversionType, innerValue);
                }

                if (conversionType == typeof(Guid))
                {
                    if (Guid.TryParse(obj.ToString(), out var guid))
                        return guid;
                    return null;
                }

                if (conversionType == typeof(Version))
                {
                    if (Version.TryParse(obj.ToString(), out var version))
                        return version;
                    return null;
                }

                return !(obj is IConvertible) ? obj : Convert.ChangeType(obj, conversionType);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将对象[主要是匿名对象]转换为dynamic
        /// </summary>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            var type = value.GetType();
            var properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var val = property.GetValue(value);
                if (property.PropertyType.FullName != null && property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
                {
                    var dval = val.ToDynamic();
                    expando.Add(property.Name, dval);
                }
                else
                {
                    expando.Add(property.Name, val);
                }
            }

            return (ExpandoObject)expando;
        }

        /// <summary> 异常信息格式化 </summary>
        /// <param name="ex"></param>
        /// <param name="isHideStackTrace"></param>
        /// <returns></returns>
        public static string Format(this Exception ex, bool isHideStackTrace = false)
        {
            var sb = new StringBuilder();
            var count = 0;
            var appString = string.Empty;
            while (ex != null)
            {
                if (count > 0)
                {
                    appString += "  ";
                }
                sb.AppendLine($"{appString}异常消息：{ex.Message}");
                sb.AppendLine($"{appString}异常类型：{ex.GetType().FullName}");
                sb.AppendLine($"{appString}异常方法：{(ex.TargetSite == null ? null : ex.TargetSite.Name)}");
                sb.AppendLine($"{appString}异常源：{ex.Source}");
                if (!isHideStackTrace && ex.StackTrace != null)
                {
                    sb.AppendLine($"{appString}异常堆栈：{ex.StackTrace}");
                }
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"{appString}内部异常：");
                    count++;
                }
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

        /// <summary> 获取最初的异常信息 </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Exception GetInnermostException(this Exception exception)
        {
            if (exception == null)
                return null;
            var inner = exception;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            return inner;
        }

        /// <summary> unescape </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UnEscape(this object value)
        {
            if (value == null)
                return string.Empty;
            var type = value.GetType();
            //枚举值
            if (type.IsEnum)
                return value.CastTo(0).ToString();
            //布尔值
            if (type == typeof(bool))
                return ((bool)value ? 1 : 0).ToString();

            var sb = new StringBuilder();
            var str = value.ToString();
            var len = str.Length;
            var i = 0;
            while (i != len)
            {
                sb.Append(Uri.IsHexEncoding(str, i) ? Uri.HexUnescape(str, ref i) : str[i++]);
            }

            return sb.ToString();
        }

        public static object GetService(this IServiceProvider provider, Type type, string name)
        {
            var services = provider.GetServices(type);
            return services.First(t => t.GetType().PropName() == name);
        }

        public static T GetService<T>(this IServiceProvider provider, string name)
        {
            var services = provider.GetServices<T>();
            return services.First(t => t.GetType().PropName() == name);
        }

        /// <summary> 模型验证 </summary>
        /// <param name="obj"></param>
        /// <param name="items"></param>
        public static void Validate(this object obj, Dictionary<object, object> items = null)
        {
            if (obj == null) return;
            var validationContext = new ValidationContext(obj, items);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obj, validationContext, results, true);

            if (isValid) return;
            var error = results.First();
            throw new ArgumentNullException(error.MemberNames.FirstOrDefault(), error.ErrorMessage);
        }

        public static string GetStringResult(this object value)
        {
            if (value == null) return string.Empty;
            if (value.GetType().IsSimpleType())
            {
                if (value is DateTime date)
                    return date.ToString("yyyy-MM-dd HH:mm:ss");
                return value.ToString();
            }
            return JsonHelper.ToJson(value);
        }

        private static readonly object ConsoleSync = new object();

        public static void Print(this IEnumerable<PrintItem> prints)
        {
            lock (ConsoleSync)
            {
                foreach (var item in prints)
                {
                    ConsoleColor? tc = null;
                    if (item.Color.HasValue)
                    {
                        tc = Console.ForegroundColor;
                        Console.ForegroundColor = item.Color.Value;
                    }

                    var content = item.Message == null
                        ? "NULL"
                        : (item.Message.GetType().IsSimpleType()
                            ? item.Message.ToString()
                            : JsonHelper.ToJson(item.Message, NamingType.CamelCase, true));
                    if (item.NewLine)
                        Console.WriteLine(content);
                    else
                        Console.Write(content);
                    if (tc.HasValue)
                        Console.ForegroundColor = tc.Value;
                }
            }
        }

        public static void Print(this object msg, ConsoleColor? color = null, bool newline = true)
        {
            new[] { new PrintItem(msg, color, newline) }.Print();
        }

        public static void PrintSucc(this object msg, bool newline = true)
        {
            msg.Print(ConsoleColor.Green, newline);
        }

        public static void PrintInfo(this object msg, bool newline = true)
        {
            msg.Print(ConsoleColor.Cyan, newline);
        }

        public static void PrintWarn(this object msg, bool newline = true)
        {
            msg.Print(ConsoleColor.Yellow, newline);
        }

        public static void PrintError(this object msg, bool newline = true)
        {
            msg.Print(ConsoleColor.Red, newline);
        }

        public static void PrintFatal(this object msg, bool newline = true)
        {
            msg.Print(ConsoleColor.Magenta, newline);
        }
    }

    public class PrintItem
    {
        public object Message { get; set; }
        public ConsoleColor? Color { get; set; }
        public bool NewLine { get; set; }

        public PrintItem(object msg, ConsoleColor? color = null, bool newline = true)
        {
            Message = msg;
            Color = color;
            NewLine = newline;
        }
    }
}