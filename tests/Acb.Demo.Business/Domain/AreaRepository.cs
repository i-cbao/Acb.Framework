using Acb.Core.Data.Adapters;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Demo.Business.Domain.Entities;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Demo.Business.Domain
{
    public class AreaRepository : DapperRepository<TAreas>
    {
        public AreaRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        public async Task<IEnumerable<TAreas>> QueryAreaAsync(string parentCode = null)
        {
            var type = typeof(TAreas);
            var sql = $"select {type.Columns()} from {type.PropName()} where parent_code=@parentCode";
            sql = Connection.FormatSql(sql);
            return await Connection.QueryAsync<TAreas>(sql, new { parentCode });
            //var str = "dapper:default:ConnectionString".Config<string>();
            //using (var conn = new MySqlConnection(str))
            //{
            //    sql = conn.FormatSql(sql);
            //    return conn.Query<TAreas>(sql, new { parentCode });
            //}
        }
    }
}
