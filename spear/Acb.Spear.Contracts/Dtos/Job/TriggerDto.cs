using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.Contracts.Dtos.Job
{
    public class TriggerDto : TriggerInputDto
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        /// <summary> 上次执行 </summary>
        public DateTime? PrevTime { get; set; }
        /// <summary> 下次执行 </summary>
        public DateTime? NextTime { get; set; }
        /// <summary> 状态 </summary>
        public TriggerStatus Status { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
