using Acb.Core.Extensions;
using System;

namespace Acb.Core.EventBus
{
    /// <summary> 消息编解码器 </summary>
    public interface IMessageCodec
    {
        /// <summary> 编码 </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] Encode(object message);

        /// <summary> 解码 </summary>
        /// <param name="data"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        object Decode(byte[] data, Type dataType = null);
    }

    /// <summary> 编码器扩展 </summary>
    public static class MessageCodecExtensions
    {
        /// <summary> 解码 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codec"></param>
        /// <param name="data"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T Decode<T>(this IMessageCodec codec, byte[] data, T def = default(T))
        {
            if (data == null || data.Length == 0)
                return def;
            if (typeof(T) == typeof(byte[]))
                return (T)(object)data;
            var message = codec.Decode(data, typeof(T));
            return message.CastTo<T>();
        }
    }
}
