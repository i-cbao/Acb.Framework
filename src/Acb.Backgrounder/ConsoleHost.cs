using System;

namespace Acb.Backgrounder
{
    /// <summary> 控制台启动项 </summary>
    public class ConsoleHost
    {
        public JobManager Manager { get; }

        private readonly IJob[] _jobs;
        /// <summary> 命令事件(返回true,退出任务) </summary>
        public event Func<string, bool> OnCommand;

        public ConsoleHost(IJob[] jobs)
        {
            _jobs = jobs;
            Manager = CreateJobWorkersManager();
        }

        public void Start()
        {
            Manager.Start();
            var key = Console.ReadLine();
            while (!string.Equals(key, "exit", StringComparison.CurrentCultureIgnoreCase))
            {
                var result = OnCommand?.Invoke(key);
                if (result.HasValue && result.Value)
                {
                    break;
                }
                key = Console.ReadLine();
            }
            Console.WriteLine("bye");
            ShutDown();
        }

        private void ShutDown()
        {
            Manager.Dispose();
        }

        private JobManager CreateJobWorkersManager()
        {
            var coordinator = new SingleServerJobCoordinator();
            return new JobManager(_jobs, coordinator);
        }
    }
}
