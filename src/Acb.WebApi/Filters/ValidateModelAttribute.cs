using Acb.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Acb.WebApi.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly bool _validation;

        public ValidateModelAttribute(bool validation = true)
        {
            _validation = validation;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_validation || context.ModelState.IsValid)
                return;
            var errordict = context.ModelState.FirstOrDefault(t => t.Value.Errors.Count > 0);
            if (errordict.Key == null)
                return;
            var value = $"{errordict.Value.Errors[0].ErrorMessage}";
            context.Result = new JsonResult(DResult.Error(value, 20012));
            base.OnActionExecuting(context);
        }
    }
}
