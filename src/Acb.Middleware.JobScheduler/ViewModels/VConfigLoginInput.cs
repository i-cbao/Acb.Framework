using System.ComponentModel.DataAnnotations;

namespace Acb.Middleware.JobScheduler.ViewModels
{
    public class VConfigLoginInput
    {
        [Required(ErrorMessage = "登陆账号不能为空")]
        public string Account { get; set; }
        [Required(ErrorMessage = "登陆密码不能为空")]
        public string Password { get; set; }
    }
}
