using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Core.Domain;
using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using Dapper;

namespace Acb.Spear.Business.Database.Services
{
    public class SqliteService : BaseService
    {
        public SqliteService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Provider = ProviderType.SQLite;
        }

        protected override Task<IEnumerable<TableDto>> QueryTableAsync()
        {
            const string sql =
                "SELECT name As Name,(CASE WHEN upper(TYPE) = 'VIEW' THEN 'View' ELSE 'Table' END ) AS Type,'' AS Description FROM sqlite_master WHERE TYPE='table' OR TYPE = 'view'";
            return Connection.QueryAsync<TableDto>(sql);
        }

        protected override Task<IEnumerable<ColumnDto>> QueryColumnAsync(string table, int? tableId = null)
        {
            const string sql =
                "Select name as Name, Lower(type) AS DbType,NOT [NotNull] AS IsNullable, PK AS IsPrimaryKey,'' as Description From Pragma_Table_Info(@table)";
            return Connection.QueryAsync<ColumnDto>(sql, new { table });
        }
    }
}
