using Acb.Core.Logging;
using System.Threading.Tasks;

namespace Acb.Core.Monitor
{
    public class LoggerMonitor : IMonitor
    {
        public async Task Record(MonitorData data)
        {
            await Task.Run(() =>
            {
                var logger = LogManager.Logger<LoggerMonitor>();
                logger.Info(data.ToString());
            });

        }
    }
}
