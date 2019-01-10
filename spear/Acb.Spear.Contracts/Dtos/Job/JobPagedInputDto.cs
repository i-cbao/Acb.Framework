using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.Contracts.Dtos.Job
{
    public class JobPagedInputDto : PageInputDto
    {
        public Guid? ProjectId { get; set; }
        public string Keyword { get; set; }
        public JobStatus? Status { get; set; }
    }
}
