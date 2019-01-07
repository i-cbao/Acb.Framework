using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> t_job </summary>
    [Naming("t_job")]
    public class TJob : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 任务名称 </summary>
        public string Name { get; set; }

        ///<summary> 任务分组 </summary>
        public string Group { get; set; }

        ///<summary> 任务状态 </summary>
        public int Status { get; set; }

        ///<summary> 任务类型 </summary>
        public int Type { get; set; }

        ///<summary> 任务描述 </summary>
        public string Desc { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreationTime { get; set; }

        ///<summary> 项目ID </summary>
        public Guid ProjectId { get; set; }
    }
}
