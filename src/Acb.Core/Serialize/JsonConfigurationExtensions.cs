using Microsoft.Extensions.Configuration;
using System.IO;

namespace Acb.Core.Serialize
{
    public static class JsonConfigurationExtensions
    {
        public static IConfigurationBuilder AddJson(this IConfigurationBuilder builder, string json)
        {
            var parser = new JsonConfigurationParser();
            return builder.AddInMemoryCollection(parser.Parse(json));
        }

        public static IConfigurationBuilder AddJson(this IConfigurationBuilder builder, Stream input)
        {
            var parser = new JsonConfigurationParser();
            return builder.AddInMemoryCollection(parser.Parse(input));
        }
    }
}
