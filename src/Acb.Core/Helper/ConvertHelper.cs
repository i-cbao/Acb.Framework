using Acb.Core.Extensions;
using System;
using System.Globalization;
using System.Reflection;

namespace Acb.Core.Helper
{
    /// <summary>
    /// 类型转换辅助
    /// </summary>
    public static class ConvertHelper
    {
        private static readonly DateTime DefaultTime = DateTime.Parse("1900-01-01");
        /// <summary>
        /// string转换为float
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns></returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
                return defValue;

            float intValue = defValue;
            bool isFloat = RegexHelper.IsFloat(strValue);
            if (isFloat)
                float.TryParse(strValue, out intValue);
            return intValue;
        }

        /// <summary>
        /// object转化为float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static float StrToFloat(object obj, float defValue)
        {
            if (obj == null)
                return defValue;
            return StrToFloat(obj.ToString(), defValue);
        }

        /// <summary>
        /// string转化为int
        /// </summary>
        /// <param name="str">字符</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static int StrToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 ||
                !RegexHelper.IsFloat(str.Trim()))
                return defValue;

            int rv;
            if (Int32.TryParse(str, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(str, defValue));
        }

        /// <summary>
        /// object转化为int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int StrToInt(object obj, int defValue)
        {
            if (obj == null)
                return defValue;
            return StrToInt(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return dateTime;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime StrToDateTime(object obj, DateTime defValue)
        {
            if (obj == null) return defValue;
            return StrToDateTime(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DefaultTime);
        }

        /// <summary> 相同属性不同类转换 </summary>
        /// <typeparam name="T">转换目标类</typeparam>
        /// <returns></returns>
        public static T ClassConvert<T>(object source)
            where T : new()
        {
            var target = new T();
            var sourceType = source.GetType();
            var targetType = target.GetType();
            var targetProps =
                targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            foreach (PropertyInfo targetProp in targetProps)
            {
                var sourceProp = sourceType.GetProperty(targetProp.Name);
                if (sourceProp == null || sourceProp.PropertyType != targetProp.PropertyType)
                    continue;
                var pv = sourceProp.GetValue(source, null);
                targetProp.SetValue(target, pv, null);
            }
            return target;
        }

        /// <summary>
        /// 获取数字中文
        /// 不完善
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ToBigNumber(int num)
        {
            var word = new[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            var dw = new[] { "十", "百", "千", "万", "亿" };
            var desc = string.Empty;
            var str = num.ToString(CultureInfo.InvariantCulture).Reverse();
            //1000,000)
            for (var i = 0; i < str.Length; i++)
            {
                var n = (StrToInt(str[i], 0));
                if (n > 0 || (i > 0 && (i - 4) % 4 == 0))
                {
                    if ((i - 1) % 4 == 0)
                        desc = dw[0] + desc;
                    else if ((i - 2) % 4 == 0)
                        desc = dw[1] + desc;
                    else if ((i - 3) % 4 == 0)
                        desc = dw[2] + desc;
                    else if (i > 3 && (i - 4) % 8 == 0)
                        desc = dw[3] + desc;
                    else if (i > 7 && i % 8 == 0)
                        desc = dw[4] + desc;
                }
                if ((i == 0 && n == 0) || (n == 0 && (desc.StartsWith(dw[3]) || desc.StartsWith(dw[4]))))
                    continue;
                if (!desc.StartsWith(word[0]) && (n != 0 || ((i - 4) % 4 != 0)))
                    desc = word[n] + desc;
            }
            return desc.TrimEnd(word[0].ToCharArray());
        }
    }
}
