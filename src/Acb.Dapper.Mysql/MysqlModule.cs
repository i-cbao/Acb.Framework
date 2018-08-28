using Acb.Core;
using Acb.Core.Data.Adapters;
using Acb.Core.Modules;

namespace Acb.Dapper.Mysql
{
    [DependsOn(typeof(CoreModule))]
    public class MySqlModule : DModule
    {
        public override void Initialize()
        {
            DbConnectionManager.AddAdapter(new MySqlConnectionAdapter());
            base.Initialize();
        }
    }
}
