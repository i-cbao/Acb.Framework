using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Database
{
    public interface IDbService
    {
        string DbName { get; }
        ProviderType Provider { get; }
        Task<IEnumerable<TableDto>> GetTablesAsync();
    }
}
