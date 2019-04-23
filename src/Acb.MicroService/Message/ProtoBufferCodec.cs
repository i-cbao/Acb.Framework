using Acb.Core.Message;
using ProtoBuf;
using System;
using System.IO;

namespace Acb.MicroService.Message
{
    public class ProtoBufferCodec : IMessageCodec
    {
        public byte[] Encode(object message)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        public object Decode(byte[] data, Type dataType)
        {
            if (data == null)
                return null;
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize(dataType, stream);
            }
        }
    }
}
