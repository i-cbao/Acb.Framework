using Acb.Core.Extensions;
using Acb.Core.Message;
using System;

namespace Acb.MicroService.Message
{
    public class MessagePackCodec : IMessageCodec
    {
        public byte[] Encode(object message, bool compress = false)
        {
            if (message == null) return new byte[0];
            byte[] buffer;
            if (message.GetType() == typeof(byte[]))
                buffer = (byte[])message;
            else
            {
                buffer = MessagePack.MessagePackSerializer.Serialize(message);
            }

            if (compress) buffer = buffer.Zip().Result;
            return buffer;
        }

        public object Decode(byte[] data, Type dataType, bool compress = false)
        {
            if (data == null || data.Length == 0)
                return null;
            if (compress) data = data.UnZip().Result;
            return MessagePack.MessagePackSerializer.NonGeneric.Deserialize(dataType, data);
        }
    }
}
