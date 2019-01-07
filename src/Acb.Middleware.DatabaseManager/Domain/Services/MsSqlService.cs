using Acb.Core.Domain;
using Acb.Middleware.DatabaseManager.Domain.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Middleware.DatabaseManager.Domain.Enums;
using Column = Acb.Middleware.DatabaseManager.Domain.Models.Column;

namespace Acb.Middleware.DatabaseManager.Domain.Services
{
    public class MsSqlService : BaseService
    {
        public MsSqlService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Provider = DbProvider.SqlServer;
        }

        protected override Task<IEnumerable<Table>> QueryTableAsync()
        {
            const string sql =
                "Select Obj.object_id As Id,Obj.name As Name,(Case Obj.type When 'V' Then 'View' Else 'Table' End) As Type," +
                "EPObj.value As [Description] From Sys.Objects Obj " +
                "Left Join Sys.Extended_Properties EPObj On EPObj.major_id = Obj.object_id And EPObj.minor_id = 0 And EPObj.name = 'MS_Description' " +
                "Where Obj.type In('U','V') And Obj.is_ms_shipped = 0";
            return Connection.QueryAsync<Table>(sql);
        }

        protected override Task<IEnumerable<Column>> QueryColumnAsync(string table, int? tableId = null)
        {
            const string sql =
                "Select Col.column_id As Id, Col.object_id As TableId,Col.name As Name,Tp.name As DbType," +
                "Col.max_length As DataLength,EPCol.value As Description,Col.is_nullable As IsNullable," +
                "Col.is_identity As AutoIncrement,IsNull(" +
                "(Select Top 1 1 From INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                "Where Table_Name = @table And Col.name = Column_Name),0) IsPrimaryKey " +
                "From Sys.Columns Col Left Join Sys.Extended_Properties EPCol On EPCol.major_id = Col.object_id " +
                "And EPCol.minor_id = Col.column_id Left Join Sys.Types Tp On Tp.system_type_id = Col.system_type_id " +
                "Where TP.name != 'sysname' And Col.object_id = @tableId Order By Col.column_id Asc";
            return Connection.QueryAsync<Column>(sql, new { table, tableId });
        }
    }
}
