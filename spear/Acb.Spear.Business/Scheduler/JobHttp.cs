using Acb.Core.Helper.Http;
using Acb.Spear.Contracts.Dtos.Job;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Scheduler
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
                    return HttpMethod.Options;
                default:
                    return HttpMethod.Get;
            }
        }

        /// <summary> 执行任务 </summary>
        /// <param name="data"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override async Task ExecuteJob(HttpDetailDto data, JobRecordDto record)
        {
            var req = new HttpRequest(data.Url)
            {
                BodyType = (HttpBodyType)data.BodyType,
                Headers = new Dictionary<string, string>
                {
                    {"Request-By", "spear"}
                }
            };
            if (!string.IsNullOrWhiteSpace(data.Data))
                req.Data = JsonConvert.DeserializeObject(data.Data);
            if (data.Header != null && data.Header.Any())
            {
                foreach (var header in data.Header)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }
            var resp = await HttpHelper.Instance.RequestAsync(GetHttpMethod(data.Method), req);
            var html = await resp.Content.ReadAsStringAsync();
            record.ResultCode = (int)resp.StatusCode;
            record.Result = html;
        }
    }
}
