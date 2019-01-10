using System;
using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.Contracts.Dtos.Job
{
    public class JobInputDto : DDto
    {
        /// <summary> 任务名 </summary>
        public string Name { get; set; }
        /// <summary> 组名 </summary>
        public string Group { get; set; }
        /// <summary> 任务描述 </summary>
        public string Desc { get; set; }
        /// <summary> 类型:0,http </summary>
        public JobType Type { get; set; }
        /// <summary> 项目ID </summary>
        public Guid? ProjectId { get; set; }
        /// <summary> 任务详情 </summary>
        public HttpDetailDto Detail { get; set; }
    }
}
