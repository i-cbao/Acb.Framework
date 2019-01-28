using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;
using System.Collections.Generic;

namespace Acb.Spear.Contracts.Dtos.Database
{
    public class DatabaseTablesDto : DDto
    {
        public string Name { get; set; }
        public string DbName { get; set; }
        public ProviderType Provider { get; set; }
        public IEnumerable<TableDto> Tables { get; set; }
    }
}
