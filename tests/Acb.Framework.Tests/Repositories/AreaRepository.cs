using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using Acb.Dapper;
using Acb.Dapper.Domain;

namespace Acb.Framework.Tests.Repositories
{
    [Naming(NamingType.UrlCase)]
    internal class TAreas : IEntity
    {
        /// <summary>城市编码</summary>

        public string CityCode { get; set; }
        /// <summary>城市名字</summary>

        public string CityName { get; set; }
        /// <summary>深度</summary>

        public int Deep { get; set; }

        /// <summary>父级</summary>
        public string ParentCode { get; set; }
    }

    internal class AreaRepository : DapperRepository<TAreas>
    {
        public TAreas Get(string code)
        {
            return Connection.QueryById<TAreas>(code, "city_code");
        }
    }
}
