using Acb.Core.Extensions;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.ViewModels.Jobs
{
    public class VJob
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
        /// <summary> 状态描述 </summary>
        public string StatusCn => Status.GetText();

        /// <summary> 上次执行 </summary>
        public DateTime? PrevTime { get; set; }
        /// <summary> 下次执行 </summary>
        public DateTime? NextTime { get; set; }

        /// <summary> 类型:0,http </summary>
        public JobType Type { get; set; }

        public string TypeCn => Type.GetText();
        /// <summary> 创建时间 </summary>
        public DateTime CreationTime { get; set; }
        /// <summary> 任务详情 </summary>
        public HttpDetailDto Detail { get; set; }
    }
}
