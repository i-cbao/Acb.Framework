using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Acb.Core.Helper.Http
{
    /// <summary> Http请求类 </summary>
    public class HttpRequest
    {
        /// <summary> URL </summary>
        public string Url { get; set; }
        /// <summary> URL参数 </summary>
        public object Params { get; set; }
        /// <summary> Header </summary>
        public IDictionary<string, string> Headers { get; set; }
        /// <summary> Body参数 </summary>
        public object Data { get; set; }
        /// <summary> 编码 </summary>
        public Encoding Encoding { get; set; }
        /// <summary> 文件列表 </summary>
        public Dictionary<string, FileStream> Files;
        /// <summary> Body类型 </summary>
        public HttpBodyType BodyType { get; set; }
        /// <summary> 超时时间 </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary> ctor </summary>
        public HttpRequest()
        {
            BodyType = HttpBodyType.Json;
            Headers = new Dictionary<string, string>();
            Encoding = Encoding.UTF8;
        }

        /// <summary> ctor </summary>
        /// <param name="url"></param>
        public HttpRequest(string url)
            : this()
        {
            Url = url;
        }
    }
}
