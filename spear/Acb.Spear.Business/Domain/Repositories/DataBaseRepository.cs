using Acb.Core;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Domain.Repositories
{
    public class DataBaseRepository : DapperRepository<TDatabase>
    {
        public async Task<PagedList<DatabaseDto>> PagedListAsync(Guid accountId, ProviderType? type = null, int page = 1,
            int size = 10)
        {
            var tableType = typeof(TDatabase);
            SQL sql = $"SELECT {tableType.Columns()} FROM [{tableType.PropName()}] WHERE [AccountId]=@accountId";
            if (type.HasValue)
            {
                sql += "AND [Provider]=@type";
            }

            sql += "ORDER BY [CreateTime] DESC";

            return await sql.PagedListAsync<DatabaseDto>(Connection, page, size, new
            {
                accountId,
                type
            });
        }

        public async Task<int> UpdateAsync(Guid id, string name, ProviderType type, string connectionString)
        {
            return await Connection.UpdateAsync(new TDatabase
            {
                Id = id,
                Name = name,
                Provider = type.ToString(),
                ConnectionString = connectionString
            }, new[] { nameof(TDatabase.Name), nameof(TDatabase.Provider), nameof(TDatabase.ConnectionString) }, Trans);
        }

        public async Task<int> UpdateStatusAsync(Guid id, CommonStatus status)
        {
            return await Connection.UpdateAsync(new TDatabase
            {
                Id = id,
                Status = (byte)status
            }, new[] { nameof(TDatabase.Status) }, Trans);
        }
    }
}
