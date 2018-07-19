using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Acb.ConfigCenter.Filters
{
    /// <summary> 运行匿名获取 </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AllowGetAttribute : ActionFilterAttribute
    {
    }
}
