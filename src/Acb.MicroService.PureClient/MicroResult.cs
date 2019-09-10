using System;

namespace Acb.MicroService.PureClient
{
    /// <summary> 基础数据结果类 </summary>
    [Serializable]
    public class MicroResult
    {
        /// <summary> 状态 </summary>
        public bool Status => Code == 0;
        /// <summary> 状态码 </summary>
        public int Code { get; set; }
        /// <summary> 错误消息 </summary>
        public string Message { get; set; }

        private DateTime _timestamp;
        /// <summary> 时间戳 </summary>
        public DateTime Timestamp
        {
            get => _timestamp == DateTime.MinValue ? DateTime.Now : _timestamp;
            set => _timestamp = value;
        }
    }
}
