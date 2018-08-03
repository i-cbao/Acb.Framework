using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Middleware.JobScheduler.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Entities;
using Quartz;
using System;
using System.Threading.Tasks;
using Acb.Core.Dependency;
using Acb.Middleware.JobScheduler.Domain;

namespace Acb.Middleware.JobScheduler.Scheduler
{
    public abstract class JobBase<T> : IJob where T : JobDetailDto
    {
        protected readonly ILogger Logger;
        

        protected JobBase()
        {
            Logger = LogManager.Logger(typeof(JobBase<>));
        }

        protected abstract Task ExecuteJob(T data, TJobRecord record);

        public async Task Execute(IJobExecutionContext context)
        {
            var record = new TJobRecord
            {
                Id = IdentityHelper.Guid32,
                StartTime = Clock.Now
            };
            try
            {
                var data = context.Get(Constant.JobData).CastTo<T>();
                if (data == null)
                    throw new BusiException("任务数据异常");
                record.JobId = data.Id;
                await ExecuteJob(data, record);
                record.Status = 1;
            }
            catch (Exception ex)
            {
                record.Remark = ex.Message;
                if (!(ex is BusiException))
                    Logger.Error(ex.Message, ex);
                record.Status = 0;
            }
            finally
            {
                record.CompleteTime = Clock.Now;
                await CurrentIocManager.Resolve<JobRepository>().InsertRecord(record);
            }
        }
    }
}
