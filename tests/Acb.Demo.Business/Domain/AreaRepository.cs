using Acb.Core.Data;
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
        public AreaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

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

        public async Task<int> UpdateName()
        {
            return await Transaction(async () =>
            {
                var count = await Connection.UpdateAsync(new TAreas { Id = "110000", CityName = "北京市" },
                    new[] { nameof(TAreas.CityName) }, Trans);
                return count;
            });
        }

        public async Task<int> UpdateParent()
        {
            return await Transaction(async () =>
            {
                var count = await Connection.UpdateAsync(new TAreas { Id = "110000", ParentCode = "1" },
                    new[] { nameof(TAreas.ParentCode) }, Trans);
                //throw new BusiException("ex test");
                return count;
            });
        }


    }
}
