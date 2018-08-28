using Acb.Core;
using Acb.Core.Modules;
using Acb.Dapper.Adapters;

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
