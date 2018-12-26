using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Modules;

namespace Acb.Dapper.SQLite
{
    [DependsOn(typeof(CoreModule))]
    public class SqliteModule : DModule
    {
        public override void Initialize()
        {
            DbConnectionManager.AddAdapter(new SqliteConnectionAdapter());
            base.Initialize();
        }
    }
}
