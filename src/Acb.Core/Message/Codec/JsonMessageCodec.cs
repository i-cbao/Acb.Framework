using Acb.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Acb.Core.Message.Codec
{
    public class JsonMessageCodec : IMessageCodec
    {
        public byte[] Encode(object message, bool compress = false)
        {
            if (message == null) return new byte[0];
            byte[] buffer;
            if (message.GetType() == typeof(byte[]))
                buffer = (byte[])message;
            else
            {
                var json = JsonConvert.SerializeObject(message);
                buffer = Encoding.UTF8.GetBytes(json);
            }

            if (compress) buffer = buffer.Zip().Result;
            return buffer;
        }

        public object Decode(byte[] data, Type dataType, bool compress = false)
        {
            if (data == null || data.Length == 0)
                return null;
            if (compress) data = data.UnZip().Result;
            var json = Encoding.UTF8.GetString(data);
            return dataType != null
                ? JsonConvert.DeserializeObject(json, dataType)
                : JsonConvert.DeserializeObject(json);
        }
    }
}
