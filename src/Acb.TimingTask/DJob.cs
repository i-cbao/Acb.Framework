using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Acb.TimingTask
{
    public abstract class DJob : IJob, IDependency
    {
        protected readonly ILogger Logger;
        protected string JobName;

        protected DJob()
        {
            Logger = LogManager.Logger(GetType());
            JobName = GetType().PropName();
        }

        public Task Execute(IJobExecutionContext context)
        {
            Logger.Debug($"job execute -> {JobName}");
            return ExecuteAsync(context);
        }

        protected abstract Task ExecuteAsync(IJobExecutionContext context);

        protected T Resolve<T>()
        {
            return CurrentIocManager.Resolve<T>();
        }

        protected object Resolve(Type type)
        {
            return CurrentIocManager.Resolve(type);
        }
    }
}
