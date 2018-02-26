using Acb.Demo.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace Acb.WebApi.Test.ViewModels
{
    public class VDemoInput
    {
        [Required(ErrorMessage = "must demo")]
        public DemoEnums Demo { get; set; }
        [Required(ErrorMessage = "名字不能为空")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "名字应为2-5个字符")]
        public string Name { get; set; }
    }
}
