using Acb.Middleware.DatabaseManager.Domain.Enums;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Acb.Middleware.DatabaseManager.Domain.Converter
{
    public class DbTypeMap
    {
        [XmlElement("Database")]
        public List<DatabaseMap> Databases { get; set; }
    }

    public class DatabaseMap
    {
        [XmlAttribute]
        public DbProvider DbProvider { get; set; }
        [XmlAttribute]
        public Language Language { get; set; }
        [XmlElement("DbType")]
        public List<DbTypeMapLange> DbTypes { get; set; }
    }
    public class DbTypeMapLange
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string To { get; set; }
    }
}
