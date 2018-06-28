using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using Acb.Dapper;

namespace Acb.Demo.Business.Domain.Entities
{
    [Naming(NamingType.UrlCase, Name = "t_areas")]
    public class TAreas : BaseEntity<string>
    {
        [Key, Naming("city_code")]
        public override string Id { get; set; }

        ///// <summary>城市编码</summary>
        //public string CityCode => Id;
        /// <summary>城市名字</summary>

        public string CityName { get; set; }
        /// <summary>深度</summary>

        public int Deep { get; set; }

        /// <summary>父级</summary>
        public string ParentCode { get; set; }
    }
}
