using System;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Backgrounder
{
    /// <summary> 后台任务进度池 </summary>
    public class Scheduler
    {
        private readonly IEnumerable<Schedule> _schedules;

        public Scheduler(ICollection<IJob> jobs, Func<DateTime> nowThunk)
        {
            if (jobs == null || jobs.Any(j => j.Interval < TimeSpan.Zero))
            {
                throw new ArgumentException("A job cannot have a negative interval.", "jobs");
            }
            _schedules = jobs.Select(job => new Schedule(job, nowThunk)).ToList();
        }

        public Scheduler(ICollection<IJob> jobs)
            : this(jobs, () => DateTime.Now)
        {
        }

        /// <summary> 获取下一个运行任务 </summary>
        /// <returns></returns>
        public Schedule Next()
        {
            var schedules = _schedules.OrderBy(s => s.NextRunTime);
            return schedules.First();
        }

        public override string ToString()
        {
            var list = _schedules.OrderBy(t => t.NextRunTime).Select(t => new
            {
                name = t.Job.Name,
                next = t.NextRunTime
            });
            return list.Aggregate(string.Empty,
                (c, t) => c + (string.Format("Task:{0},NextRunTime:{1}{2}", t.name, t.next, Environment.NewLine)));
        }
    }
}
