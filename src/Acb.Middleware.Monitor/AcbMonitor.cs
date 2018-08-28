using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Monitor;
using Acb.Core.Timing;
using Acb.Middleware.Monitor.Domain;
using Acb.Middleware.Monitor.Domain.Entities;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Middleware.Monitor
{
    internal class AcbMonitor : IMonitor
    {
        /// <summary> unescape </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string UnEscape(object value)
        {
            if (value == null)
                return string.Empty;
            var type = value.GetType();
            //枚举值
            if (type.IsEnum)
                return value.CastTo(0).ToString();
            //布尔值
            if (type == typeof(bool))
                return ((bool)value ? 1 : 0).ToString();

            var sb = new StringBuilder();
            var str = value.ToString();
            var len = str.Length;
            var i = 0;
            while (i != len)
            {
                sb.Append(Uri.IsHexEncoding(str, i) ? Uri.HexUnescape(str, ref i) : str[i++]);
            }

            return sb.ToString();
        }

        public async Task Record(string service, string url, string @from, long milliseconds, string data = null, string userAgent = null,
            string clientIp = null)
        {
            var model = new TMonitor
            {
                Id = IdentityHelper.Guid32,
                Service = service,
                Url = url,
                Data = UnEscape(data ?? string.Empty),
                Referer = from,
                UserAgent = userAgent,
                ClientIp = clientIp,
                Time = milliseconds,
                CreateTime = Clock.Now
            };
            await CurrentIocManager.Resolve<MonitorRepository>().InsertAsync(model);
        }
    }
}
