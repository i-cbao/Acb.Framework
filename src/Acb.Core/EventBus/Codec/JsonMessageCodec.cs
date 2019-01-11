using Newtonsoft.Json;
using System;
using System.Text;

namespace Acb.Core.EventBus.Codec
{
    public class JsonMessageCodec : IMessageCodec
    {
        public byte[] Encode(object message)
        {
            if (message == null) return new byte[0];
            if (message.GetType() == typeof(byte[]))
                return (byte[])message;
            var json = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(json);
        }

        public object Decode(byte[] data, Type dataType = null)
        {
            if (data == null || data.Length == 0)
                return null;
            var json = Encoding.UTF8.GetString(data);
            return dataType != null
                ? JsonConvert.DeserializeObject(json, dataType)
                : JsonConvert.DeserializeObject(json);
        }
    }
}
