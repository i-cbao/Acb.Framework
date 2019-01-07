using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> 帐号表 </summary>
    [Naming("t_account")]
    public class TAccount : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 帐号 </summary>
        public string Account { get; set; }

        ///<summary> 密码 </summary>
        public string Password { get; set; }

        ///<summary> 密码盐 </summary>
        public string PasswordSalt { get; set; }

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
