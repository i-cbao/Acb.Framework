using Acb.Core.Logging;
using System;
using System.Threading.Tasks;

namespace Acb.Core.Monitor
{
    /// <summary> 默认监控 </summary>
    public class DMonitor : IMonitor
    {
        /// <summary> 记录数据 </summary>
        /// <param name="service"></param>
        /// <param name="url"></param>
        /// <param name="from"></param>
        /// <param name="milliseconds"></param>
        /// <param name="data"></param>
        /// <param name="userAgent"></param>
        /// <param name="clientIp"></param>
        public Task Record(string service, string url, string from, long milliseconds, string data = null, string userAgent = null,
            string clientIp = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    MonitorManager.Each(async m => await m.Record(service, url, from, milliseconds, data, userAgent, clientIp));
                }
                catch (Exception ex)
                {
                    LogManager.Logger<DMonitor>().Error(ex.Message, ex);
                }
            });
        }
    }
}
