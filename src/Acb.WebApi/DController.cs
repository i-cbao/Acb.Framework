using Acb.Core;
using Acb.Core.Serialize;
using Acb.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace Acb.WebApi
{
    [ValidateModel]
    [ActionTiming]
    public abstract class DController : Controller
    {
        protected HttpContext Current => AcbHttpContext.Current;

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

        #region Results

        protected DResult Success => DResult.Success;

        protected DResult Error(string message, int code = -1)
        {
            return DResult.Error(message, code);
        }

        protected DResult<T> Succ<T>(T data)
        {
            return DResult.Succ(data);
        }

        protected DResult<T> Error<T>(string message, int code = -1)
        {
            return DResult.Error<T>(message, code);
        }

        protected DResults<T> Succ<T>(IEnumerable<T> list, int count = -1)
        {
            return DResult.Succ(list, count);
        }

        protected DResults<T> Errors<T>(string message, int code = -1)
        {
            return DResult.Errors<T>(message, code);
        }

        #endregion
    }
}
