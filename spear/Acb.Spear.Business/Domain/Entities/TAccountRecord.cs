using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> t_account_record </summary>
    [Naming("t_account_record")]
    public class TAccountRecord : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 帐号ID </summary>
        public Guid AccountId { get; set; }

        ///<summary> 状态 </summary>
        public byte Status { get; set; }

        ///<summary> 备注 </summary>
        public string Remark { get; set; }

        ///<summary> IP </summary>
        public string CreateIp { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 客户端信息 </summary>
        public string UserAgent { get; set; }
    }
}
