using System;
using Acb.Core.Extensions;

namespace Acb.Core.Message
{
    /// <summary> 消息编解码器 </summary>
    public interface IMessageCodec
    {
        /// <summary> 编码 </summary>
        /// <param name="message"></param>
        /// <param name="compress"></param>
        /// <returns></returns>
        byte[] Encode(object message, bool compress = false);

        /// <summary> 解码 </summary>
        /// <param name="data"></param>
        /// <param name="dataType"></param>
        /// <param name="compress"></param>
        /// <returns></returns>
        object Decode(byte[] data, Type dataType, bool compress = false);
    }

    /// <summary> 编码器扩展 </summary>
    public static class MessageCodecExtensions
    {
        /// <summary> 解码 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codec"></param>
        /// <param name="data"></param>
        /// <param name="def"></param>
        /// <param name="compress"></param>
        /// <returns></returns>
        public static T Decode<T>(this IMessageCodec codec, byte[] data, T def = default(T), bool compress = false)
        {
            if (data == null || data.Length == 0)
                return def;
            if (typeof(T) == typeof(byte[]))
                return (T)(object)data;
            var message = codec.Decode(data, typeof(T), compress);
            return message.CastTo<T>();
        }
    }
}
