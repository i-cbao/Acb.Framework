using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Modules;
using Acb.Core.Monitor;
using System;

namespace Acb.Middleware.Monitor
{
    [DependsOn(typeof(CoreModule))]
    public class MonitorModule : DModule
    {
        public override void Initialize()
        {
            var enable = Environment.GetEnvironmentVariable("ENABLE_MONITOR").CastTo(false);
            if (enable)
            {
                MonitorManager.Add(new AcbMonitor());
            }

            base.Initialize();
        }
    }
}
