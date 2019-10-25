using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Monitor;
using Acb.Core.Timing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Acb.WebApi.Filters
{
    /// <summary> 日志过滤器 </summary>
    public class RecordFilter : ActionFilterAttribute, IExceptionFilter
    {
        private const string ItemKey = "action_record_{0}";
        private readonly string _type;

        public RecordFilter(string type = null)
        {
            _type = string.IsNullOrWhiteSpace(type) ? MonitorModules.Action : type;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var raw = $"{request.Scheme}://{request.Host}{request.Path.Value}{request.QueryString.Value}";
            var dto = new MonitorData
            {
                Service = _type,
                Url = raw,
                Method = context.HttpContext.Request.Method
            };
            var req = context.HttpContext.Request;
            if (req.Method != HttpMethods.Get)
            {
                if (req.ContentType == "application/x-www-form-urlencoded")
                {
                    var dict = req.Form.ToDictionary(k => k.Key, v => v.Value.ToString());
                    dto.Data = JsonConvert.SerializeObject(dict);
                }
                else
                {
                    context.HttpContext.Request.EnableBuffering();
                    var body = context.HttpContext.Request.Body;
                    if (body.CanSeek)
                    {
                        body.Seek(0, SeekOrigin.Begin);
                    }

                    using (var ms = new MemoryStream())
                    {
                        await body.CopyToAsync(ms);
                        ms.Position = 0;
                        using (var stream = new StreamReader(ms))
                        {
                            dto.Data = await stream.ReadToEndAsync();
                        }
                    }
                }
            }
            else
            {
                dto.Data = req.QueryString.ToUriComponent();
            }

            var key = string.Format(ItemKey, _type);
            context.HttpContext.Items.Add(key, dto);
            await base.OnActionExecutionAsync(context, next);
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var manager = context.HttpContext.RequestServices.GetService<MonitorManager>();
            MonitorData dto;
            var key = string.Format(ItemKey, _type);
            if (manager != null && context.HttpContext.Items.TryGetValue(key, out var value) &&
                (dto = value as MonitorData) != null)
            {
                var result = await next();
                dto.CompleteTime = Clock.Now;
                if (result.Exception != null)
                {
                    dto.Code = (int)HttpStatusCode.InternalServerError;
                    dto.Result = result.Exception.Message;
                }
                else
                {
                    dto.Code = (int)HttpStatusCode.OK;
                    switch (result.Result)
                    {
                        case ObjectResult obj:
                            dto.Result = JsonConvert.SerializeObject(obj.Value);
                            break;
                        case ContentResult content:
                            dto.Result = content.Content;
                            break;
                        case JsonResult json:
                            dto.Result = JsonConvert.SerializeObject(json.Value);
                            break;
                        default:
                            dto.Result = "success";
                            break;
                    }
                }
                context.HttpContext.RequestServices.Monitor(dto);
            }
            else
            {
                await next();
            }
        }

        /// <inheritdoc />
        /// <summary> 异常处理 </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            MonitorData dto;
            if (!context.HttpContext.Items.TryGetValue(ItemKey, out var value) ||
                (dto = value as MonitorData) == null)
                return;
            dto.Code = (int)HttpStatusCode.InternalServerError;
            dto.CompleteTime = Clock.Now;
            if (context.Exception != null)
            {
                if (context.Exception is BusiException busi)
                {
                    if (busi.Code != -1)
                        dto.Code = busi.Code;
                }

                dto.Result = context.Exception.Message;
            }
            context.HttpContext.RequestServices.Monitor(dto);
        }
    }
}
