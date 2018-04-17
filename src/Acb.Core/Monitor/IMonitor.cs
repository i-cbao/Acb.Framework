using System.Threading.Tasks;

namespace Acb.Core.Monitor
{
    /// <summary> 监控接口 </summary>
    public interface IMonitor
    {
        /// <summary> 记录数据 </summary>
        /// <param name="service"></param>
        /// <param name="url"></param>
        /// <param name="from"></param>
        /// <param name="milliseconds"></param>
        /// <param name="data"></param>
        /// <param name="userAgent"></param>
        /// <param name="clientIp"></param>
        Task Record(string service, string url, string from, long milliseconds, string data = null,
            string userAgent = null, string clientIp = null);
    }
}
