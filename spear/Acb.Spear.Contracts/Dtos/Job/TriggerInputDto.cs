using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.Contracts.Dtos.Job
{
    public class TriggerInputDto : DDto
    {
        /// <summary> 触发器类型 </summary>
        public TriggerType Type { get; set; }
        /// <summary> Corn表达式 </summary>
        public string Corn { get; set; }
        /// <summary> 执行次数 </summary>
        public int Times { get; set; }
        /// <summary> 时间间隔(秒) </summary>
        public int Interval { get; set; }
        /// <summary> 开始时间 </summary>
        public DateTime? Start { get; set; }
        /// <summary> 结束时间 </summary>
        public DateTime? Expired { get; set; }
    }
}
