using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Middleware.JobScheduler.Domain.Entities
{
    /// <summary> Http任务 </summary>
    [Naming("t_job_http")]
    public class TJobHttp : BaseEntity<string>
    {
        /// <summary> URL </summary>
        public string Url { get; set; }
        /// <summary> 请求方式 </summary>
        public int Method { get; set; }
        /// <summary> 数据类型 </summary>
        public int BodyType { get; set; }
        /// <summary> 请求头 </summary>
        public string Header { get; set; }
        /// <summary> 数据 </summary>
        public string Data { get; set; }
    }
}
