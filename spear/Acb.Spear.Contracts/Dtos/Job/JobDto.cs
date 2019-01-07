using System;
using System.Collections.Generic;
using Acb.Spear.Domain.Enums;

namespace Acb.Spear.Contracts.Dtos.Job
{
    public class JobDto
    {
        public Guid Id { get; set; }
        /// <summary> 任务名 </summary>
        public string Name { get; set; }
        /// <summary> 组名 </summary>
        public string Group { get; set; }
        /// <summary> 任务描述 </summary>
        public string Desc { get; set; }
        /// <summary> 状态 </summary>
        public JobStatus Status { get; set; }
        /// <summary> 类型:0,http </summary>
        public JobType Type { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreationTime { get; set; }
        /// <summary> 任务详情 </summary>
        public HttpDetailDto Detail { get; set; }
        /// <summary> 触发器 </summary>
        public List<TriggerDto> Triggers { get; set; }
    }
}
