using Acb.Core.Extensions;
using Acb.Core.Message;
using ProtoBuf;
using System;
using System.IO;

namespace Acb.MicroService.Message
{
    public class ProtoBufferCodec : IMessageCodec
    {
        public byte[] Encode(object message, bool compress = false)
        {
            if (message == null) return new byte[0];
            byte[] buffer;
            if (message.GetType() == typeof(byte[]))
                buffer = (byte[])message;
            else
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, message);
                    buffer = stream.ToArray();
                }
            }

            if (compress) buffer = buffer.Zip().Result;
            return buffer;
        }

        public object Decode(byte[] data, Type dataType, bool compress = false)
        {
            if (data == null || data.Length == 0)
                return null;
            if (compress) data = data.UnZip().Result;
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize(dataType, stream);
            }
        }
    }
}
