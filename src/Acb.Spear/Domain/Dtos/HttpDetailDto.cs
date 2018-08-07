namespace Acb.Middleware.JobScheduler.Domain.Dtos
{
    public class HttpDetailDto : JobDetailDto
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
