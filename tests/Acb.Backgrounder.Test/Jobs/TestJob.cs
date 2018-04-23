using Acb.Core.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Backgrounder.Test.Jobs
{
    public class TestJob : Job
    {
        [ThreadStatic]
        private int _count;

        public TestJob(string name, TimeSpan interval, TimeSpan? timeout = null, DateTime? start = null,
            DateTime? expire = null) : base(name, interval, timeout, start, expire)
        {
        }

        public override Task Execute()
        {
            _count++;
            var logger = LogManager.Logger<TestJob>();
            logger.Info($"hello {Name}!,{_count}");
            Thread.CurrentThread.Join(TimeSpan.FromSeconds(_count));
            return Task.CompletedTask;
        }
    }
}
