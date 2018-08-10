using System.Collections.Generic;
using Acb.Core.Helper.Http;
using Acb.Spear.Domain.Dtos;
using Acb.Spear.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acb.Spear.Scheduler
{
    public class JobHttp : JobBase<HttpDetailDto>
    {
        private static HttpMethod GetHttpMethod(int method)
        {
            switch (method)
            {
                case 0:
                    return HttpMethod.Get;
                case 1:
                    return HttpMethod.Post;
                case 2:
                    return HttpMethod.Delete;
                case 3:
                    return HttpMethod.Put;
                case 4:
                    return HttpMethod.Patch;
                default:
                    return HttpMethod.Get;
            }
        }

        /// <summary> 执行任务 </summary>
        /// <param name="data"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override async Task ExecuteJob(HttpDetailDto data, TJobRecord record)
        {
            var req = new HttpRequest(data.Url)
            {
                BodyType = (HttpBodyType)data.BodyType
            };
            if (!string.IsNullOrWhiteSpace(data.Data))
                req.Data = JsonConvert.DeserializeObject(data.Data);
            if (!string.IsNullOrWhiteSpace(data.Header))
            {
                req.Headers = JsonConvert.DeserializeObject<IDictionary<string, string>>(data.Header);
            }
            var resp = await HttpHelper.Instance.RequestAsync(GetHttpMethod(data.Method), req);
            var html = await resp.Content.ReadAsStringAsync();
            record.Result = $"code:{resp.StatusCode},content:{html}";
        }
    }
}
