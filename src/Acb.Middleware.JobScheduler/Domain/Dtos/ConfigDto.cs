using System;
using Acb.Core.Domain.Dtos;

namespace Acb.Middleware.JobScheduler.Domain.Dtos
{
    public class ConfigDto : DDto
    {
        public string Id { get; set; }
        public string Md5 { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public object Config { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
