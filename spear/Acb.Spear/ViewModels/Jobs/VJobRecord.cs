using Acb.Core.Extensions;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.ViewModels.Jobs
{
    public class VJobRecord
    {
        public Guid Id { get; set; }
        /// <summary> 状态 </summary>
        public RecordStatus Status { get; set; }
        /// <summary> 状态描述 </summary>
        public string StatusCn => Status.GetText();
        /// <summary> 开始时间 </summary>
        public DateTime StartTime { get; set; }
        /// <summary> 完成时间 </summary>
        public DateTime CompleteTime { get; set; }
        /// <summary> 耗时(ms) </summary>
        public double Time => (CompleteTime - StartTime).TotalMilliseconds;
        /// <summary> 执行结果 </summary>
        public string Result { get; set; }
        /// <summary> 状态码 </summary>
        public int ResultCode { get; set; }
        /// <summary> 备注 </summary>
        public string Remark { get; set; }
    }
}
