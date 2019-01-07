using Acb.Spear.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acb.Spear.ViewModels
{
    public class VConfigProjectEditInput : IValidatableObject
    {
        /// <summary> 项目名称 </summary>
        [Required(ErrorMessage = "请输入项目名称")]
        [StringLength(12, MinimumLength = 2, ErrorMessage = "项目名称应为2-12个字符")]
        public string Name { get; set; }

        /// <summary> 安全性 </summary>
        public SecurityEnum Security { get; set; }
        /// <summary> 登陆账号 </summary>
        [StringLength(32, MinimumLength = 3, ErrorMessage = "登陆账号应为3-32位字符")]
        public string Account { get; set; }
        /// <summary> 密码 </summary>
        [StringLength(32, MinimumLength = 6, ErrorMessage = "登录密码应为6-32位字符")]
        public string Password { get; set; }
        /// <summary> 描述 </summary>
        public string Desc { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (Security == SecurityEnum.None)
                return results;
            if (string.IsNullOrWhiteSpace(Account))
                results.Add(new ValidationResult("请输入登陆账号"));
            if (string.IsNullOrWhiteSpace(Password))
                results.Add(new ValidationResult("请输入登陆密码"));
            return results;
        }
    }
    public class VConfigProjectInput : VConfigProjectEditInput
    {
        /// <summary> 项目编码 </summary>
        [StringLength(8, MinimumLength = 2, ErrorMessage = "项目编码应为2-8个字符")]
        public string Code { get; set; }
    }
}
