using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core
{
    /// <summary> 基础数据结果类 </summary>
    [Serializable]
    public class DResult
    {
        public bool Status { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public DResult(bool status, string message, int code = 0)
        {
            Status = status;
            Message = message;
            Code = code;
        }

        public DResult(string message, int code = -1)
            : this(false, message, code)
        {
        }

        public static DResult Success
        {
            get { return new DResult(true, string.Empty); }
        }

        public static DResult Error(string message, int code = -1)
        {
            return new DResult(false, message, code);
        }

        public static DResult<T> Succ<T>(T data)
        {
            return new DResult<T>(true, data);
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

        public DResult(bool status, T data, int code = 0)
            : base(status, string.Empty, code)
        {
            Data = data;
        }

        public DResult(string message, int code = -1)
            : base(false, message, code)
        {
        }
    }

    [Serializable]
    public class DResults<T> : DResult
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }

        public DResults(string message, int code = -1)
            : base(false, message, code)
        {
        }

        public DResults(IEnumerable<T> list)
            : base(true, string.Empty)
        {
            var data = list as T[] ?? list.ToArray();
            Data = data;
            TotalCount = data.Length;
        }

        public DResults(IEnumerable<T> list, int totalCount)
            : base(true, string.Empty)
        {
            Data = list;
            TotalCount = totalCount;
        }
    }
}