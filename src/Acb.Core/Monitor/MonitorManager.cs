using Acb.Core.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Core.Monitor
{
    /// <summary> 监控管理 </summary>
    public class MonitorManager
    {
        private readonly IEnumerable<IMonitor> _monitors;

        public MonitorManager(IEnumerable<IMonitor> monitors)
        {
            _monitors = monitors;
        }

        internal void Record(MonitorData data)
        {
            if (_monitors == null || !_monitors.Any())
                return;
            var config = MonitorConfig.Config();
            if (!config.Enable)
                return;
            if (config.Modules.ContainsKey(data.Service) && !config.Modules[data.Service])
                return;
            Task.Run(() =>
            {
                foreach (var monitor in _monitors)
                {
                    monitor.Record(data);
                }
            });
        }

        public void MonitorAction(Action<MonitorData> action, string service = null)
        {
            var data = new MonitorData(service);
            try
            {
                action.Invoke(data);
            }
            catch (Exception ex)
            {
                data.Code = 500;
                data.Result = ex.Message;
            }
            finally
            {
                data.CompleteTime = Clock.Now;
                Record(data);
            }
        }
    }
}
