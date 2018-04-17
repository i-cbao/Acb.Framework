using System.Threading.Tasks;
using Acb.Core.Helper;
using Acb.Core.Monitor;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Domain;

namespace Acb.Middleware.Monitor
{
    internal class AcbMonitor : DapperRepository<TMonitor>, IMonitor
    {
        public async Task Record(string service, string url, string from, long milliseconds, string data = null, string userAgent = null,
            string clientIp = null)
        {
            var model = new TMonitor
            {
                Id = IdentityHelper.Guid32,
                Service = service,
                Url = url,
                Data = data,
                Referer = from,
                UserAgent = userAgent,
                ClientIp = clientIp,
                Time = milliseconds,
                CreateTime = Clock.Now
            };
            using (var conn = GetConnection("icb_main"))
            {
                await conn.InsertAsync(model);
            }
        }
    }
}
