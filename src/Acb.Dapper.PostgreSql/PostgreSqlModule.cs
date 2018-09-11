using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Modules;

namespace Acb.Dapper.PostgreSql
{
    [DependsOn(typeof(CoreModule))]
    public class PostgreSqlModule : DModule
    {
        public override void Initialize()
        {
            DbConnectionManager.AddAdapter(new PostgreSqlAdapter());
            base.Initialize();
        }
    }
}
