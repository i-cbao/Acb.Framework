using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Modules;
using Acb.Core.Monitor;

namespace Acb.Middleware.Monitor
{
    [DependsOn(typeof(CoreModule))]
    public class MonitorModule : DModule
    {
        public override void Initialize()
        {
            if (Consts.Mode != ProductMode.Dev)
            {
                MonitorManager.Add(new AcbMonitor());
            }

            base.Initialize();
        }
    }
}
