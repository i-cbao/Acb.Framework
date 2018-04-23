using System;

namespace Acb.Backgrounder
{
    /// <summary> 后台任务进度 </summary>
    public class Schedule : IDisposable
    {
        readonly Func<DateTime> _nowThunk;

        public Schedule(IJob job)
            : this(job, () => DateTime.UtcNow)
        {
        }

        public Schedule(IJob job, Func<DateTime> nowThunk)
        {
            Job = job ?? throw new ArgumentNullException("job");
            _nowThunk = nowThunk;
            _lastRunTime = Job.StartTime ?? nowThunk();
        }

        /// <summary> 后台任务 </summary>
        public IJob Job { get; private set; }

        /// <summary> 最后一次运行时间 </summary>
        private DateTime _lastRunTime;

        /// <summary> 下一次运行时间 </summary>
        public DateTime NextRunTime
        {
            get
            {
                var next = _lastRunTime.Add(Job.Interval);
                if (Job.ExpireTime.HasValue && Job.ExpireTime < next)
                    return DateTime.MaxValue;
                return next;
            }
        }

        /// <summary> 获取距离下一次运行的时间 </summary>
        /// <returns></returns>
        public TimeSpan GetIntervalToNextRun()
        {
            var now = _nowThunk();
            if (NextRunTime < now)
            {
                return TimeSpan.FromMilliseconds(1);
            }
            return NextRunTime - now;
        }

        /// <summary> 任务运行完成 </summary>
        private void SetRunComplete()
        {
            _lastRunTime = _nowThunk();
        }

        void IDisposable.Dispose()
        {
            SetRunComplete();
        }
    }
}
