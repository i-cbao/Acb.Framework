using System;
using Quartz;
using System.Threading.Tasks;
using Acb.Core.Logging;

namespace Acb.Backgrounder.Test.Jobs
{
    public class TestJob : Quartz.IJob
    {
        private readonly ILogger _logger;

        public TestJob()
        {
            _logger = LogManager.Logger<TestJob>();
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.Info("test job..");
            return Task.CompletedTask;
        }
    }
}
