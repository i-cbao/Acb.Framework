using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Adapters;
using Acb.Dapper.Domain;
using Acb.Demo.Business.Domain.Entities;
using Dapper;
using System.Collections.Generic;

namespace Acb.Demo.Business.Domain
{
    public class AreaRepository : DapperRepository<TAreas>
    {
        public AreaRepository() : base("default") { }
        public IEnumerable<TAreas> QueryArea(string parentCode = null)
        {
            var type = typeof(TAreas);
            var sql = $"select {type.Columns()} from {type.PropName()} where parent_code=@parentCode";
            sql = Connection.FormatSql(sql);
            return Connection.Query<TAreas>(sql, new { parentCode });
            //var str = "dapper:default:ConnectionString".Config<string>();
            //using (var conn = new MySqlConnection(str))
            //{
            //    sql = conn.FormatSql(sql);
            //    return conn.Query<TAreas>(sql, new { parentCode });
            //}
        }
    }
}
