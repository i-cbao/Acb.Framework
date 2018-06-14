using Acb.Core.Extensions;
using Acb.Core.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Acb.Core;

namespace Acb.WebApi.ViewModels
{
    /// <inheritdoc />
    /// <summary> 参数签名基类 </summary>
    public class VSignature : IValidatableObject
    {
        private static readonly string SignKey = "app_sign_key".Config<string>();

        /// <summary> 签名 </summary>
        [Required(ErrorMessage = "接口签名无效")]
        [StringLength(45, ErrorMessage = "接口签名无效")]
        public string Sign { get; set; }

        private static string UnEscape(object value)
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

        /// <inheritdoc />
        /// <summary> 参数验证 </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(SignKey))
                return results;
            if (Sign.Length != 45)
            {
                results.Add(new ValidationResult("参数签名格式异常"));
                return results;
            }

            var timestamp = Sign.Substring(0, 13).CastTo(0L);
            //有效期验证 2分钟有效期
            if (DateTime.Now > timestamp.FromMillisecondTimestamp().AddMinutes(2))
            {
                results.Add(new ValidationResult("请求已失效"));
                return results;
            }

            //签名验证
            //获取除Sign外所有参数并进行排序、url拼接
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dict = props.Where(prop => prop.Name != nameof(Sign))
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(validationContext.ObjectInstance));
            var array = new SortedDictionary<string, object>(dict).Select(t => $"{t.Key}={UnEscape(t.Value)}");

            // 规则 Sign=时间戳(毫秒) + Md532(除Sign外所有参数的url拼接(如：a=1&b=2,不编码) + key + 时间戳(毫秒)).ToLower()
            var unSigned = string.Concat(string.Join("&", array), SignKey, timestamp);
            var sign = timestamp + Core.Helper.EncryptHelper.MD5(unSigned);
            //签名验证
            if (!string.Equals(sign, Sign, StringComparison.CurrentCultureIgnoreCase))
                results.Add(new ValidationResult("参数签名验证失败"));
            return results;
        }
    }
}
