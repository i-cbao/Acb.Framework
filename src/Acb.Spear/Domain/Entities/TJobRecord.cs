using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Domain.Entities
{
    [Naming("t_job_record")]
    public class TJobRecord : BaseEntity<string>
    {
        /// <summary> 任务ID </summary>
        public string JobId { get; set; }
        /// <summary> 状态 </summary>
        public int Status { get; set; }
        /// <summary> 开始时间 </summary>
        public DateTime StartTime { get; set; }
        /// <summary> 完成时间 </summary>
        public DateTime CompleteTime { get; set; }
        /// <summary> 执行结果 </summary>
        public string Result { get; set; }
        /// <summary> 备注 </summary>
        public string Remark { get; set; }
    }
}
