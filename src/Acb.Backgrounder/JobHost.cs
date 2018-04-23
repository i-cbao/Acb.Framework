using System;
using System.Threading.Tasks;

namespace Acb.Backgrounder
{
    /// <summary> 后台任务主机 </summary>
    public class JobHost : IJobHost
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
        }

        public void DoWork(Task work)
        {
            if (work == null)
            {
                throw new ArgumentNullException("work");
            }
            lock (_lock)
            {
                if (_shuttingDown)
                {
                    return;
                }

                if (work.Status == TaskStatus.Created)
                {
                    work.Start();
                }
                work.Wait();
            }
        }
    }
}
