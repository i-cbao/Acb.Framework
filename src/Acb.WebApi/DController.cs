using Acb.Core;
using Acb.Core.Serialize;
using Acb.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace Acb.WebApi
{
    /// <summary> 控制器基类 </summary>
    [ValidateModel]
    public abstract class DController : Controller
    {
        /// <summary> 当前请求上下文 </summary>
        protected HttpContext Current => AcbHttpContext.Current;

        /// <summary> 从body中读取对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NonAction]
        protected T FromBody<T>()
        {
            if (Current == null) return default(T);
            var input = Current.Request.Body;
            input.Seek(0, SeekOrigin.Begin);
            using (var stream = new StreamReader(input))
            {
                var body = stream.ReadToEnd();
                return JsonHelper.Json<T>(body, NamingType.CamelCase);
            }
        }

        /// <summary> 身份验证 </summary>
        /// <param name="context"></param>
        [NonAction]
        public virtual void AuthorizeValidate(HttpContext context) { }

        #region Results

        /// <summary> 成功 </summary>
        protected DResult Success => DResult.Success;

        /// <summary> 失败 </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected DResult Error(string message, int code = -1)
        {
            return DResult.Error(message, code);
        }

        /// <summary> 成功 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected DResult<T> Succ<T>(T data)
        {
            return DResult.Succ(data);
        }

        /// <summary> 失败 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected DResult<T> Error<T>(string message, int code = -1)
        {
            return DResult.Error<T>(message, code);
        }

        /// <summary> 成功 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected DResults<T> Succ<T>(IEnumerable<T> list, int count = -1)
        {
            return DResult.Succ(list, count);
        }

        /// <summary> 失败 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected DResults<T> Errors<T>(string message, int code = -1)
        {
            return DResult.Errors<T>(message, code);
        }

        #endregion
    }
}
