﻿using Acb.Core;
using Acb.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Acb.WebApi.Filters
{
    /// <summary> 模型验证过滤器 </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly bool _validation;

        /// <summary> 构造函数 </summary>
        /// <param name="validation"></param>
        public ValidateModelAttribute(bool validation = true)
        {
            _validation = validation;
        }

        /// <summary> 请求执行时验证模型 </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_validation || context.ModelState.IsValid)
                return;
            var errordict = context.ModelState.FirstOrDefault(t => t.Value.Errors.Count > 0);
            if (errordict.Key == null)
                return;
            var value = errordict.Value.Errors[0].ErrorMessage;
            context.Result = new JsonResult(DResult.Error(value, ErrorCodes.ParamaterError));
            base.OnActionExecuting(context);
        }
    }
}
