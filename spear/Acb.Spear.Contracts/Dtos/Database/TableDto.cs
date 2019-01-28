using Acb.Spear.Contracts.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Spear.Contracts.Dtos.Database
{
    public class TableDto : DConverted
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? Id { get; set; }

        /// <summary> 表名 </summary>
        public override string Name { get; set; }

        public TableType Type { get; set; }

        /// <summary> 主键列 </summary>
        public ColumnDto PrimaryColumn { get { return Columns.FirstOrDefault(m => m.IsPrimaryKey); } }

        /// <summary> 是否有重命名 </summary>
        public bool HasConvertedName { get { return Columns.Any(m => m.Name != m.ConvertedName); } }

        /// <summary> 是否有自增列 </summary>
        public bool HasAutoIncrement { get { return Columns.Any(m => m.AutoIncrement); } }

        /// <summary> 描述 </summary>
        public string Description { get; set; }

        public IEnumerable<ColumnDto> Columns { get; set; }
    }
}
