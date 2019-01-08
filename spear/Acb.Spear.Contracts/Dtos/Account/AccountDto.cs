using Acb.Core.Domain.Dtos;
using System;

namespace Acb.Spear.Contracts.Dtos.Account
{
    public class AccountDto : DDto
    {
        ///<summary> Id </summary>
        public Guid Id { get; set; }

        ///<summary> 帐号 </summary>
        public string Account { get; set; }

        ///<summary> 昵称 </summary>
        public string Nick { get; set; }

        ///<summary> 头像 </summary>
        public string Avatar { get; set; }

        ///<summary> 角色 </summary>
        public short Role { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 最后登录时间 </summary>
        public DateTime? LastLoginTime { get; set; }

        ///<summary> 项目ID </summary>
        public Guid? ProjectId { get; set; }
    }
}
