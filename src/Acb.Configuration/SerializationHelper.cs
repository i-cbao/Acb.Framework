using Acb.Core.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Acb.Configuration
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(Stream stream, ILogger logger = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            try
            {
                using (JsonReader jsonReader = new JsonTextReader(new StreamReader(stream)))
                    return (T)new JsonSerializer().Deserialize(jsonReader, typeof(T));
            }
            catch (Exception ex)
            {
                logger?.Error("Serialization exception: {0}", (object)ex);
            }
            return default(T);
        }
    }
}
