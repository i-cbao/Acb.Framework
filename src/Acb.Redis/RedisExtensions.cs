using Acb.Core.Extensions;
using Acb.Core.Serialize;
using StackExchange.Redis;
using System;

namespace Acb.Redis
{
    public static class RedisExtensions
    {
        #region 私有方法
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static RedisValue Serialize(object obj)
        {
            if (obj == null)
            {
                return RedisValue.Null;
            }
            var type = obj.GetType();
            return type == typeof(string) || type.IsValueType ? obj.ToString() : JsonHelper.ToJson(obj);

            //var binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    binaryFormatter.Serialize(memoryStream, obj);
            //    return memoryStream.ToArray();
            //}
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static T Deserialize<T>(RedisValue value)
        {
            if (!value.HasValue)
            {
                return default(T);
            }
            var type = typeof(T);
            return type == typeof(string) || type.IsValueType ? value.ToString().CastTo<T>() : JsonHelper.Json<T>(value);
            //var binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream(stream))
            //{
            //    return binaryFormatter.Deserialize(memoryStream).CastTo<T>();
            //}
        }
        #endregion

        public static T Get<T>(this IDatabase database, string key)
        {
            var value = database.StringGet(key);
            return Deserialize<T>(value);
        }

        public static object Get(this IDatabase database, string key)
        {
            return database.Get<object>(key);
        }

        public static void Set<T>(this IDatabase database, string key, T value, TimeSpan? expired = null)
        {
            database.StringSet(key, Serialize(value), expired);
        }
    }
}
