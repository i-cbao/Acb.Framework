using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Spear.Domain.Entities
{
    /// <summary> 配置表 </summary>
    [Naming("t_config")]
    public class TConfig : BaseEntity<string>
    {
        /// <summary> 名称 </summary>
        public string Name { get; set; }
        /// <summary> 模式 </summary>
        public string Mode { get; set; }
        /// <summary> 配置内容 </summary>
        public string Content { get; set; }
        /// <summary> 时间戳 </summary>
        public DateTime Timestamp { get; set; }
        /// <summary> 项目Id </summary>
        public string ProjectCode { get; set; }
        /// <summary> 校验码 </summary>
        public string Md5 { get; set; }
        /// <summary> 描述 </summary>
        public string Desc { get; set; }
        /// <summary> 状态 </summary>
        public byte Status { get; set; }
    }
}
