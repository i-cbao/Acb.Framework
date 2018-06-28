using Acb.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acb.Framework.Tests.EfCore.Entities
{
    [Table("t_areas")]
    public class TAreas : BaseEntity<string>
    {
        [Key, Column("city_code")]
        public override string Id { get; set; }

        ///// <summary>城市编码</summary>
        //public string CityCode => Id;
        /// <summary>城市名字</summary>
        [Column("city_name")]
        public string CityName { get; set; }
        /// <summary>深度</summary>
        [Column("deep")]
        public int Deep { get; set; }

        /// <summary>父级</summary>
        [Column("parent_code")]
        public string ParentCode { get; set; }
    }
}
