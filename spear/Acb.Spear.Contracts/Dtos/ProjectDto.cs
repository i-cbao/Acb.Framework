using System;
using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.Contracts.Dtos
{
    public class ProjectDto : DDto
    {
        ///<summary> Id </summary>
        public Guid Id { get; set; }

        ///<summary> 项目名称 </summary>
        public string Name { get; set; }

        ///<summary> 安全性:0,匿名;1.获取验证;2,管理验证 </summary>
        public SecurityEnum Security { get; set; }

        ///<summary> 描述 </summary>
        public string Desc { get; set; }

        ///<summary> 项目编码 </summary>
        public string Code { get; set; }
        ///<summary> 项目密钥 </summary>
        public string Secret { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 状态 </summary>
        public byte Status { get; set; }
    }
}
