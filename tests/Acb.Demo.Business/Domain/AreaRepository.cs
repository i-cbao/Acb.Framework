using Acb.Core.Domain;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Demo.Business.Domain.Entities;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Dapper.Adapters;

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
        }

        public async Task<TAreas> Get(string code)
        {
            return await Connection.QueryByIdAsync<TAreas>(code);
        }
    }
}
