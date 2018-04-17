using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Acb.Core.Monitor
{
    /// <summary> 监控管理 </summary>
    public static class MonitorManager
    {
        /// <summary> 监控列表 </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IMonitor> Monitors;

        static MonitorManager()
        {
            Monitors = new ConcurrentDictionary<RuntimeTypeHandle, IMonitor>();
        }

        /// <summary> 添加适配 </summary>
        public static void Add(IMonitor monitor)
        {
            var key = monitor.GetType().TypeHandle;
            if (Monitors.ContainsKey(key)) return;
            Monitors.TryAdd(key, monitor);
        }

        /// <summary> 获取监控 </summary>
        /// <returns></returns>
        public static DMonitor Monitor()
        {
            return new DMonitor();
        }

        internal static void Each(Action<IMonitor> monitorAction)
        {
            if (Monitors == null || !Monitors.Any())
                return;
            foreach (var monitor in Monitors)
            {
                monitorAction?.Invoke(monitor.Value);
            }
        }
    }
}
