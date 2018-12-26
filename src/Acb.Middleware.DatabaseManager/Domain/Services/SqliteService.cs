using Acb.Core.Domain;
using Acb.Middleware.DatabaseManager.Domain.Enums;
using Acb.Middleware.DatabaseManager.Domain.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Column = Acb.Middleware.DatabaseManager.Domain.Models.Column;

namespace Acb.Middleware.DatabaseManager.Domain.Services
{
    public class SqliteService : BaseService
    {
        public SqliteService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Provider = DbProvider.SQLite;
        }

        protected override Task<IEnumerable<Table>> QueryTableAsync()
        {
            const string sql =
                "SELECT name As Name,(CASE WHEN upper(TYPE) = 'VIEW' THEN 'View' ELSE 'Table' END ) AS Type,'' AS Description FROM sqlite_master WHERE TYPE='table' OR TYPE = 'view'";
            return Connection.QueryAsync<Table>(sql);
        }

        protected override Task<IEnumerable<Column>> QueryColumnAsync(string table, int? tableId = null)
        {
            const string sql =
                "Select name as Name, Lower(type) AS DbType,NOT [NotNull] AS IsNullable, PK AS IsPrimaryKey,'' as Description From Pragma_Table_Info(@table)";
            return Connection.QueryAsync<Column>(sql, new { table });
        }
    }
}
