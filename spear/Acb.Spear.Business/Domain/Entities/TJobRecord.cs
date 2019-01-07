using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> t_job_record </summary>
    [Naming("t_job_record")]
    public class TJobRecord : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 任务ID </summary>
        public Guid JobId { get; set; }

        ///<summary> 状态 </summary>
        public int Status { get; set; }

        ///<summary> 开始时间 </summary>
        public DateTime StartTime { get; set; }

        ///<summary> 结束时间 </summary>
        public DateTime CompleteTime { get; set; }

        ///<summary> 执行结果 </summary>
        public string Result { get; set; }

        ///<summary> 备注 </summary>
        public string Remark { get; set; }
    }
}
