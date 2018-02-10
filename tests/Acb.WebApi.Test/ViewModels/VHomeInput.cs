using System.ComponentModel.DataAnnotations;

namespace Acb.WebApi.Test.ViewModels
{
    public class VHomeInput
    {
        [Required(ErrorMessage = "Must Code")]
        public string Code { get; set; }
    }
}
