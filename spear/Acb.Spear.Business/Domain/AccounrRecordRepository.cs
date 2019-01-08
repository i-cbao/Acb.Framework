using Acb.Core;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Domain
{
    public class AccounrRecordRepository : DapperRepository<TAccountRecord>
    {
        public Task<PagedList<TAccountRecord>> QueryPagedListAsync(Guid accountId, int page = 1, int size = 10)
        {
            var type = typeof(TAccountRecord);
            SQL sql =
                $"SELECT {type.Columns()} FROM [{type.PropName()}] WHERE [AccountId]=@accountId ORDER BY [CreateTime] DESC";
            return sql.PagedListAsync<TAccountRecord>(Connection, page, size, new { accountId });
        }
    }
}
