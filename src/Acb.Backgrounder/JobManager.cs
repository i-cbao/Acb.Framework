using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Acb.Backgrounder
{
    /// <summary> 后台任务管理者 </summary>
    public class JobManager : IDisposable
    {
        private readonly IJobHost _host;
        private readonly Timer _timer;
        private readonly IJobCoordinator _coordinator;
        private readonly Scheduler _scheduler;
        private readonly IEnumerable<IJob> _jobs;
        /// <summary> 异常事件 </summary>
        public event Action<Exception> OnException;
        /// <summary> 日志事件 </summary>
        public event Action<string> OnLog;
        private bool _isCancel;

        /// <summary> 任务失败后是否重新启动 </summary>
        public bool RestartSchedulerOnFailure { private get; set; }

        public JobManager(IEnumerable<IJob> jobs)
            : this(jobs, new JobHost(), new SingleServerJobCoordinator())
        {
        }
        public JobManager(IEnumerable<IJob> jobs, IJobHost host)
            : this(jobs, host, new SingleServerJobCoordinator()) { }

        public JobManager(IEnumerable<IJob> jobs, IJobCoordinator coordinator)
            : this(jobs, new JobHost(), coordinator) { }

        public JobManager(IEnumerable<IJob> jobs, IJobHost host, IJobCoordinator coordinator)
        {
            _jobs = jobs ?? throw new ArgumentNullException(nameof(jobs));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));

            _scheduler = new Scheduler(jobs.ToArray());
            _timer = new Timer(OnTimerElapsed);
        }

        public void Start()
        {
            _isCancel = false;
            _timer.Next(TimeSpan.FromMilliseconds(1));
        }

        public void Stop()
        {
            _isCancel = true;
            _timer.Stop();
        }

        private void OnTimerElapsed(object sender)
        {
            try
            {
                _timer.Stop();
                DoNextJob();
                if (!_isCancel)
                {
                    _timer.Next(_scheduler.Next().GetIntervalToNextRun()); // Start up again.
                }
            }
            catch (Exception e)
            {
                OnException?.Invoke(e); // Someone else's problem.

                if (RestartSchedulerOnFailure)
                {
                    _timer.Next(_scheduler.Next().GetIntervalToNextRun()); // Start up again.
                }
            }
        }

        private void DoNextJob()
        {
            using (var schedule = _scheduler.Next())
            {
                var work = _coordinator.GetWork(schedule.Job);

                if (work != null)
                {
                    _host.DoWork(work);
                }
            }

            OnLog?.Invoke(_scheduler.ToString());
        }

        public void Dispose()
        {
            Stop();
            foreach (var job in _jobs.OfType<IDisposable>())
            {
                job.Dispose();
            }
            _timer.Dispose();
            _coordinator.Dispose();
        }
    }
}
