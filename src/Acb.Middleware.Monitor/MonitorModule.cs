using Acb.Core;
using Acb.Core.Modules;

namespace Acb.Middleware.Monitor
{
    [DependsOn(typeof(CoreModule))]
    public class MonitorModule : DModule
    {
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
