using Acb.Core.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core
{
    /// <summary> 基础数据结果类 </summary>
    [Serializable]
    public class DResult
    {
        public bool Status => Code == 0;

        public int Code { get; set; }

        public string Message { get; set; }
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp == DateTime.MinValue ? Clock.Now : _timestamp;
            set => _timestamp = value;
        }

        public DResult() : this(string.Empty) { }

        public DResult(string message, int code = 0)
        {
            Message = message;
            Code = code;
        }

        /// <summary> 成功 </summary>
        public static DResult Success => new DResult(string.Empty);

        /// <summary> 错误的结果 </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DResult Error(string message, int code = -1)
        {
            return new DResult(message, code);
        }

        public static DResult<T> Succ<T>(T data)
        {
            return new DResult<T>(data);
        }

        public static DResult<T> Error<T>(string message, int code = -1)
        {
            return new DResult<T>(message, code);
        }

        public static DResults<T> Succ<T>(IEnumerable<T> data, int count = -1)
        {
            return count < 0 ? new DResults<T>(data) : new DResults<T>(data, count);
        }

        public static DResults<T> Errors<T>(string message, int code = -1)
        {
            return new DResults<T>(message, code);
        }
    }

    [Serializable]
    public class DResult<T> : DResult
    {
        public T Data { get; set; }

        public DResult() : this(default(T)) { }
        public DResult(T data)
            : base(string.Empty)
        {
            Data = data;
        }

        public DResult(string message, int code = -1)
            : base(message, code)
        {
        }
    }

    [Serializable]
    public class DResults<T> : DResult
    {
        /// <summary> 数据集合 </summary>
        public IEnumerable<T> Data { get; set; }
        /// <summary> 总数 </summary>
        public int Total { get; set; }

        /// <summary> 默认构造函数 </summary>
        public DResults() : this(string.Empty) { }

        public DResults(string message, int code = -1)
            : base(message, code)
        {
        }

        public DResults(IEnumerable<T> list)
            : base(string.Empty)
        {
            var data = list as T[] ?? list.ToArray();
            Data = data;
            Total = data.Length;
        }

        public DResults(IEnumerable<T> list, int total)
            : base(string.Empty)
        {
            Data = list;
            Total = total;
        }

        public DResults(IPagedList<T> list)
            : base(string.Empty)
        {
            var data = list as T[] ?? list.ToArray();
            Data = data;
            Total = list.Total;
        }
    }
}