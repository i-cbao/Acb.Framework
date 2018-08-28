using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Middleware.Monitor.Domain.Entities
{
    [Naming(NamingType.UrlCase, Name = "t_called_record")]
    public class TMonitor : BaseEntity<string>
    {
        public string Service { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
        public string Referer { get; set; }
        public long Time { get; set; }
        public string UserAgent { get; set; }
        public string ClientIp { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
