using Acb.Middleware.DatabaseManager.Domain.Converter;
using Acb.Middleware.DatabaseManager.Domain.Enums;

namespace Acb.Middleware.DatabaseManager.Domain.Models
{
    public class Column : DConverted
    {
        /// <summary> 编号 </summary>
        public int Id { get; set; }
        /// <summary> 列名 </summary>
        public override string Name { get; set; }

        /// <summary> 类型 </summary>
        public string DbType { get; set; }
        /// <summary> 长度 </summary>
        public long? DataLength { get; set; }
        /// <summary> 是否为主键 </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary> 是否可为空 </summary>
        public bool IsNullable { get; set; }

        /// <summary> 描述 </summary>
        public string Description { get; set; }

        /// <summary> 是否自增 </summary>
        public bool AutoIncrement { get; set; }

        public override string ConvertedName => IsPrimaryKey ? "Id" : base.ConvertedName;

        public string LanguageType(DbProvider provider, Language language) =>
            DbTypeConverter.Instance.LanguageType(provider, language, DbType, IsNullable);
    }
}
