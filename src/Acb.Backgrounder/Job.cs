using System;
using System.Threading.Tasks;

namespace Acb.Backgrounder
{
    /// <summary> 后台任务基类 </summary>
    public abstract class Job : IJob
    {
        /// <summary>
        /// 任务构造函数
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">间隔</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="start">开始时间</param>
        /// <param name="expire">失效时间</param>
        protected Job(string name, TimeSpan interval, TimeSpan? timeout = null,
            DateTime? start = null, DateTime? expire = null)
        {
            Name = name;
            Interval = interval;
            TimeOut = timeout;
            StartTime = start;
            ExpireTime = expire;
        }

        /// <summary> 名称 </summary>
        public string Name { get; private set; }

        /// <summary> 异步任务 </summary>
        /// <returns></returns>
        public abstract Task Execute();

        /// <summary> 开始时间 </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary> 截至时间 </summary>
        public DateTime? ExpireTime { get; private set; }

        /// <summary> 时间间隔 </summary>
        public TimeSpan Interval { get; private set; }

        /// <summary> 超时时间 </summary>
        public TimeSpan? TimeOut { get; private set; }
    }
}
