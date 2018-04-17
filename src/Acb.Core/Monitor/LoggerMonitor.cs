using Acb.Core.Logging;
using System.Threading.Tasks;

namespace Acb.Core.Monitor
{
    public class LoggerMonitor : IMonitor
    {
        public async Task Record(string service, string url, string from, long milliseconds, string data = null, string userAgent = null,
            string clientIp = null)
        {
            await Task.Run(() =>
            {
                var logger = LogManager.Logger<LoggerMonitor>();
                logger.Info($"[{service}],{url},{milliseconds}ms,from:{from},data:{data},{userAgent},{clientIp}");
            });

        }
    }
}
