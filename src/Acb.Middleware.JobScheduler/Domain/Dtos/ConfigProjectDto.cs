using Acb.Core.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Enums;

namespace Acb.Middleware.JobScheduler.Domain.Dtos
{
    public class ConfigProjectDto : DDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ConfigSecurity Security { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Desc { get; set; }
    }
}
