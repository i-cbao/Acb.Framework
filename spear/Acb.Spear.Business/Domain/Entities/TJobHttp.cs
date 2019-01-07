using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> t_job_http </summary>
    [Naming("t_job_http")]
    public class TJobHttp : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> Url </summary>
        public string Url { get; set; }

        ///<summary> 请求方式 </summary>
        public int Method { get; set; }

        ///<summary> 数据类型 </summary>
        public int BodyType { get; set; }

        ///<summary> 请求头 </summary>
        public string Header { get; set; }

        ///<summary> 请求数据 </summary>
        public string Data { get; set; }
    }
}
