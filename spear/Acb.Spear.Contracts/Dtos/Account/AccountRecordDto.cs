using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.Contracts.Dtos.Account
{
    public class AccountRecordDto : DDto
    {
        ///<summary> Id </summary>
        public Guid Id { get; set; }

        ///<summary> 状态 </summary>
        public AccountRecordStatus Status { get; set; }

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
