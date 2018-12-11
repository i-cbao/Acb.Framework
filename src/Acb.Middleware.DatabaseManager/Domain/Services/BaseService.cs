using Acb.Core.Domain;
using Acb.Middleware.DatabaseManager.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Middleware.DatabaseManager.Domain.Services
{
    public abstract class BaseService : IDatabaseService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected IDbConnection Connection => UnitOfWork.Connection;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public string DbName => Connection.Database;
        public string Provider { get; protected set; }

        public virtual async Task<IEnumerable<Table>> GetTablesAsync()
        {
            var tables = await QueryTableAsync();
            var tableList = tables as Table[] ?? tables.ToArray();
            foreach (var table in tableList)
            {
                var columns = await QueryColumnAsync(table.Name, table.Id);
                table.Columns = columns;
            }

            return tableList.OrderBy(t => t.Type).ThenBy(t => t.Name);
        }

        /// <summary> 查询所有表和视图 </summary>
        /// <returns></returns>
        protected abstract Task<IEnumerable<Table>> QueryTableAsync();

        /// <summary> 查询表所有字段 </summary>
        /// <param name="table"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        protected abstract Task<IEnumerable<Column>> QueryColumnAsync(string table, int? tableId = null);
    }
}
