using Acb.Core.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acb.Configuration
{
    /// <summary>
    /// Utility class for working with configuration values that have placeholders in them.
    /// A placeholder takes the form of <code> ${some:config:reference?default_if_not_present}&gt;</code>
    /// Note: This was "inspired" by the Spring class: PropertyPlaceholderHelper
    /// </summary>
    public static class PropertyPlaceholderHelper
    {
        private const string Prefix = "${";
        private const string Suffix = "}";
        private const string Separator = "?";

        /// <summary>
        /// Replaces all placeholders of the form <code> ${some:config:reference?default_if_not_present}</code>
        /// with the corresponding value from the supplied <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" />.
        /// </summary>
        /// <param name="property">the string containing one or more placeholders</param>
        /// <param name="config">the configuration used for finding replace values.</param>
        /// <param name="logger">optional logger</param>
        /// <returns>the supplied value with the placeholders replaced inline</returns>
        public static string ResolvePlaceholders(string property, IConfiguration config, ILogger logger = null)
        {
            return ParseStringValue(property, config, new HashSet<string>(), logger);
        }

        private static string ParseStringValue(string property, IConfiguration config, ISet<string> visitedPlaceHolders, ILogger logger = null)
        {
            if (config == null || string.IsNullOrEmpty(property))
                return property;
            StringBuilder stringBuilder = new StringBuilder(property);
            int num = property.IndexOf(Prefix, StringComparison.Ordinal);
            while (num != -1)
            {
                int endIndex = PropertyPlaceholderHelper.FindEndIndex(stringBuilder, num);
                if (endIndex != -1)
                {
                    string property1 = stringBuilder.Substring(num + Prefix.Length, endIndex);
                    string str1 = property1;
                    if (!visitedPlaceHolders.Add(str1))
                        throw new ArgumentException(
                            string.Format("Circular placeholder reference '{0}' in property definitions", str1));
                    string stringValue1 = ParseStringValue(property1, config, visitedPlaceHolders);
                    string index1 = stringValue1.Replace('[', ':').Replace("]", string.Empty);
                    string property2 = config[index1];
                    if (property2 == null)
                    {
                        int length = stringValue1.IndexOf(Separator, StringComparison.Ordinal);
                        if (length != -1)
                        {
                            string index2 = stringValue1.Substring(0, length);
                            string str2 = stringValue1.Substring(length + Separator.Length);
                            property2 = config[index2] ?? str2;
                        }
                    }
                    if (property2 != null)
                    {
                        string stringValue2 = PropertyPlaceholderHelper.ParseStringValue(property2, config, visitedPlaceHolders);
                        stringBuilder.Replace(num, endIndex + Suffix.Length, stringValue2);
                        if (logger != null)
                            logger.Debug("Resolved placeholder '{0}'" + stringValue1, Array.Empty<object>());
                        num = stringBuilder.IndexOf(Prefix, num + stringValue2.Length);
                    }
                    else
                        num = stringBuilder.IndexOf(Prefix, endIndex + Prefix.Length);
                    visitedPlaceHolders.Remove(str1);
                }
                else
                    num = -1;
            }
            return stringBuilder.ToString();
        }

        private static int FindEndIndex(StringBuilder property, int startIndex)
        {
            var index = startIndex + Prefix.Length;
            var num = 0;
            while (index < property.Length)
            {
                if (SubstringMatch(property, index, Suffix))
                {
                    if (num <= 0)
                        return index;
                    --num;
                    index += Suffix.Length;
                }
                else if (SubstringMatch(property, index, Prefix))
                {
                    ++num;
                    index += Prefix.Length;
                }
                else
                    ++index;
            }
            return -1;
        }

        private static bool SubstringMatch(StringBuilder str, int index, string substring)
        {
            for (var index1 = 0; index1 < substring.Length; ++index1)
            {
                var index2 = index + index1;
                if (index2 >= str.Length || (int)str[index2] != (int)substring[index1])
                    return false;
            }
            return true;
        }

        private static void Replace(this StringBuilder builder, int start, int end, string str)
        {
            builder.Remove(start, end - start);
            builder.Insert(start, str);
        }

        private static int IndexOf(this StringBuilder builder, string str, int start)
        {
            if (start >= builder.Length)
                return -1;
            return builder.ToString().IndexOf(str, start, StringComparison.Ordinal);
        }

        private static string Substring(this StringBuilder builder, int start, int end)
        {
            return builder.ToString().Substring(start, end - start);
        }
    }
}
