using System.Collections.Generic;

namespace Acb.Configuration
{
    public class ConfigEnvironment
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public IList<string> Profiles { get; set; }

        public IList<PropertySource> PropertySources { get; set; }

        public string Version { get; set; }
    }
}
