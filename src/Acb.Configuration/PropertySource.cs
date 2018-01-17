using System.Collections.Generic;

namespace Acb.Configuration
{
    public class PropertySource
    {
        public PropertySource()
        {
        }

        public PropertySource(string name, IDictionary<string, object> properties)
        {
            Name = name;
            Source = properties;
        }

        public string Name { get; set; }

        public IDictionary<string, object> Source { get; set; }
    }
}
