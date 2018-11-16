using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Modules;
using Acb.Core.Monitor;

namespace Acb.Middleware.Monitor
{
    [DependsOn(typeof(CoreModule))]
    public class MonitorModule : DModule
    {
        private const string EnableMonitor = "ENABLE_MONITOR";
        public override void Initialize()
        {
            var enable = EnableMonitor.Env(false);
            if (enable)
            {
                MonitorManager.Add(new AcbMonitor());
            }

            base.Initialize();
        }
    }
}
