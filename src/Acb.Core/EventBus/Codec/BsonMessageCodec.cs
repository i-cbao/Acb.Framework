using Newtonsoft.Json.Bson;
using System;
using System.IO;

namespace Acb.Core.EventBus.Codec
{
    public class BsonMessageCodec : IMessageCodec
    {
        public byte[] Encode(object message)
        {
            var writer = new BsonWriter(new MemoryStream());

            throw new NotImplementedException();
        }

        public object Decode(byte[] data, Type dataType = null)
        {
            throw new NotImplementedException();
        }
    }
}
