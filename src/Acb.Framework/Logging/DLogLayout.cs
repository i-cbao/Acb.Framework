using System.IO;
using System.Reflection;
using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace Acb.Framework.Logging
{
    public class DLogLayout : PatternLayout
    {
        public DLogLayout()
        {
            AddConverter("prop", typeof(DLogLayoutConverter));
        }
    }

    public class DLogLayoutConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }

        private static object LookupProperty(string property, LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            var propertyInfo = loggingEvent.MessageObject.GetType()
                .GetProperty(property, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (propertyInfo != null)
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }
    }
}
