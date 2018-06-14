using System;
using System.Threading;

namespace Acb.Backgrounder
{
    public static class Extensions
    {
        public static void Stop(this Timer timer)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void Next(this Timer timer, TimeSpan dueTime)
        {
            timer.Change(dueTime, TimeSpan.FromMilliseconds(Timeout.Infinite));
        }

        public static bool IsActive(this IWorkItem workItem)
        {
            return workItem != null && workItem.Completed == null;
        }

        public static bool IsTimedOut(this IWorkItem workItem, IJob job, Func<DateTime> nowThunk)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }
            return workItem != null && job.TimeOut.HasValue &&
                   workItem.Started.Add(job.TimeOut.Value) < nowThunk();
        }

        public static TimeSpan FromMonths(this DateTime dateTime, int months)
        {
            return dateTime.AddMonths(months) - dateTime;
        }

        public static TimeSpan FromYears(this DateTime dateTime, int years)
        {
            return dateTime.AddYears(years) - dateTime;
        }
    }
}
