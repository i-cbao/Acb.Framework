using System;
using System.ComponentModel.DataAnnotations;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.ViewModels.Account
{
    public class VAccountInput
    {
        /// <summary> 登陆账号 </summary>
        [Required(ErrorMessage = "请输入登陆账号")]
        public string Account { get; set; }
        /// <summary> 昵称 </summary>
        public string Nick { get; set; }
        /// <summary> 登陆密码 </summary>
        [Required(ErrorMessage = "请输入登陆密码")]
        public string Password { get; set; }
        /// <summary> 角色 </summary>
        public AccountRole Role { get; set; }
        /// <summary> 项目ID </summary>
        public Guid? Project { get; set; }
    }
}
