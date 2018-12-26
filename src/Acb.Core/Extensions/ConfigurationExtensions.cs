using Acb.Core.Serialize;
using Microsoft.Extensions.Configuration;
using System.IO;
using Acb.Core.Config;

namespace Acb.Core.Extensions
{
    public static class ConfigurationExtensions
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
