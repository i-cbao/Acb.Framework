using Acb.Core.Message;
using System;

namespace Acb.MicroService.Message
{
    public class MessagePackCodec : IMessageCodec
    {
        public byte[] Encode(object message)
        {
            return MessagePack.MessagePackSerializer.Serialize(message);
        }

        public object Decode(byte[] data, Type dataType)
        {
            return data == null ? null : MessagePack.MessagePackSerializer.NonGeneric.Deserialize(dataType, data);
        }
    }
}
