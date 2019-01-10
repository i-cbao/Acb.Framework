using Acb.Core.Extensions;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.ViewModels.Jobs
{
    public class VTrigger
    {
        public Guid Id { get; set; }
        /// <summary> 触发器类型 </summary>
        public TriggerType Type { get; set; }
        /// <summary> 类型描述 </summary>
        public string TypeCn => Type.GetText();
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
        /// <summary> 下次次执行 </summary>
        public DateTime? NextTime { get; set; }
        /// <summary> 状态 </summary>
        public TriggerStatus Status { get; set; }
        /// <summary> 状态描述 </summary>
        public string StatusCn => Status.GetText();
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
