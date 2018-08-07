using System;
using Acb.Middleware.JobScheduler.Domain.Enums;

namespace Acb.Middleware.JobScheduler.Domain.Dtos
{
    public class TriggerDto
    {
        public string Id { get; set; }
        public string JobId { get; set; }
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
        /// <summary> 上次执行 </summary>
        public DateTime? PrevTime { get; set; }
        /// <summary> 下次执行 </summary>
        public DateTime? NextTime { get; set; }
    }
}
