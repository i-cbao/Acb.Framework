using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Data.Adapters;
using Acb.Core.Modules;
using Acb.Dapper.Adapters;

namespace Acb.Dapper
{
    [DependsOn(typeof(CoreModule))]
    public class DapperModule : DModule
    {
        public override void Initialize()
        {
            DbConnectionManager.AddAdapter(new SqlServerAdapter());
            base.Initialize();
        }
    }
}
