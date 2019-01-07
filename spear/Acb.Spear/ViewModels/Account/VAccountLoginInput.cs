using System.ComponentModel.DataAnnotations;

namespace Acb.Spear.ViewModels
{
    public class VConfigLoginInput
    {
        /// <summary> 登录账户 </summary>
        [Required(ErrorMessage = "登陆账号不能为空")]
        public string Account { get; set; }
        /// <summary> 登录密码 </summary>
        [Required(ErrorMessage = "登陆密码不能为空")]
        public string Password { get; set; }
    }
}
