using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Domain.Entities
{
    [Naming("t_job")]
    public class TJob : BaseEntity<string>
    {
        /// <summary> 任务名 </summary>
        public string Name { get; set; }
        /// <summary> 组名 </summary>
        public string Group { get; set; }
        /// <summary> 任务描述 </summary>
        public string Desc { get; set; }
        /// <summary> 状态/// </summary>
        public int Status { get; set; }
        /// <summary> 类型:0,http </summary>
        public int Type { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreationTime { get; set; }
    }
}
