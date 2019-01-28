using Acb.Core.Domain;
using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Database.Services
{
    public abstract class BaseService : IDbService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected IDbConnection Connection => UnitOfWork.Connection;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public string DbName => Connection.Database;
        public ProviderType Provider { get; protected set; }

        public virtual async Task<IEnumerable<TableDto>> GetTablesAsync()
        {
            var tables = await QueryTableAsync();
            var tableList = tables as TableDto[] ?? tables.ToArray();
            foreach (var table in tableList)
            {
                var columns = await QueryColumnAsync(table.Name, table.Id);
                table.Columns = columns;
            }

            return tableList.OrderBy(t => t.Type).ThenBy(t => t.Name);
        }

        /// <summary> 查询所有表和视图 </summary>
        /// <returns></returns>
        protected abstract Task<IEnumerable<TableDto>> QueryTableAsync();

        /// <summary> 查询表所有字段 </summary>
        /// <param name="table"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        protected abstract Task<IEnumerable<ColumnDto>> QueryColumnAsync(string table, int? tableId = null);
    }
}
