using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Acb.TimingTask
{
    public abstract class DJob : IJob, IDependency
    {
        protected readonly ILogger Logger;

        private string _name;

        protected virtual string JobName
        {
            get => string.IsNullOrWhiteSpace(_name) ? GetType().PropName() : _name;
            set => _name = value;
        }

        protected DJob()
        {
            Logger = LogManager.Logger(GetType());
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.Debug($"job {JobName} start");
            var watcher = Stopwatch.StartNew();
            try
            {
                await ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                Logger.Error($"job {JobName} error:{ex.Message}", ex);
            }
            finally
            {
                watcher.Stop();
                Logger.Debug($" job {JobName} complete -> use {watcher.ElapsedMilliseconds}ms");
                if (context.NextFireTimeUtc.HasValue)
                {
                    Logger.Debug($"{JobName} next time:{context.NextFireTimeUtc.Value.LocalDateTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
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
