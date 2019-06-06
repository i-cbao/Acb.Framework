using System.Threading.Tasks;

namespace Acb.Core.Monitor
{
    /// <summary> 监控接口 </summary>
    public interface IMonitor
    {
        /// <summary> 记录数据 </summary>
        /// <param name="data">监控数据</param>
        Task Record(MonitorData data);
    }
}
