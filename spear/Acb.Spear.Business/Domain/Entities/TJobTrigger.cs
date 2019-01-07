using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> t_job_trigger </summary>
    [Naming("t_job_trigger")]
    public class TJobTrigger : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 任务ID </summary>
        public Guid JobId { get; set; }

        ///<summary> 触发器类型 </summary>
        public int Type { get; set; }

        ///<summary> Corn表达式 </summary>
        public string Corn { get; set; }

        ///<summary> 执行次数 </summary>
        public int? Times { get; set; }

        ///<summary> 执行间隔(秒) </summary>
        public int? Interval { get; set; }

        ///<summary> 开始时间 </summary>
        public DateTime? Start { get; set; }

        ///<summary> 结束时间 </summary>
        public DateTime? Expired { get; set; }

        ///<summary> 上次执行时间 </summary>
        public DateTime? PrevTime { get; set; }

        ///<summary> 下次执行时间 </summary>
        public DateTime? NextTime { get; set; }
    }
}
