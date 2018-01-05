using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Acb.Core.Extensions;
using System;

namespace Acb.Core.Timing
{
    public class DateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue(0);
                return;
            }
            long timestamp = 0;
            if (value is DateTime)
            {
                var dt = (DateTime)value;
                timestamp = dt.ToMillisecondsTimestamp();
            }
            else if (value is DateTimeOffset)
            {
                var dt = ((DateTimeOffset)value).DateTime;
                timestamp = dt.ToMillisecondsTimestamp();
            }
            //timestamp = timestamp < 0 ? 0 : timestamp;//时间小于1970会返回负数
            writer.WriteValue(timestamp);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!(objectType == typeof(DateTime)) && !(objectType == typeof(DateTimeOffset)) &&
                (!(objectType == typeof(DateTime?)) && !(objectType == typeof(DateTimeOffset?))))
                throw new JsonSerializationException("不是日期格式 .");
            var timestamp = (long?)reader.Value ?? 0;
            if (timestamp == 0 && objectType.IsNullableType())
                return null;
            return DateTimeHelper.FromMillisecondTimestamp(timestamp);
        }
    }
}
