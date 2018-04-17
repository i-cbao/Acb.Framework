﻿using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Middleware.Monitor
{
    [Naming(NamingType.UrlCase, Name = "t_monitor")]
    internal class TMonitor : BaseEntity<string>
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
