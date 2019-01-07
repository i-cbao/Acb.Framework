using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;
using System;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> t_project </summary>
    [Naming("t_project")]
    public class TProject : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 项目名称 </summary>
        public string Name { get; set; }

        ///<summary> 安全性:0,匿名;1,管理验证;2.获取验证; </summary>
        public short Security { get; set; }

        ///<summary> 描述 </summary>
        public string Desc { get; set; }

        ///<summary> 项目编码 </summary>
        public string Code { get; set; }

        ///<summary> 项目密钥 </summary>
        public string Secret { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 状态 </summary>
        public short Status { get; set; }
    }
}
