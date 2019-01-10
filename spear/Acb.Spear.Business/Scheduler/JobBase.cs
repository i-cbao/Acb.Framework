using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Contracts.Enums;
using Quartz;
using System;
using System.Threading.Tasks;
using Acb.Spear.Business.Domain;

namespace Acb.Spear.Business.Scheduler
{
    public abstract class JobBase<T> : IJob where T : JobDetailDto
    {
        protected readonly ILogger Logger;


        protected JobBase()
        {
            Logger = LogManager.Logger(typeof(JobBase<>));
        }

        protected abstract Task ExecuteJob(T data, JobRecordDto record);

        public async Task Execute(IJobExecutionContext context)
        {
            var record = new JobRecordDto
            {
                Id = IdentityHelper.NewSequentialGuid(),
                StartTime = Clock.Now,
                TriggerId = Guid.Parse(context.Trigger.Key.Name)
            };
            try
            {
                var data = context.JobDetail.JobDataMap.Get(Constants.JobData).CastTo<T>();
                if (data == null)
                    throw new BusiException("任务数据异常");
                record.JobId = data.Id;
                await ExecuteJob(data, record);
                record.Status = RecordStatus.Success;
            }
            catch (Exception ex)
            {
                record.Remark = ex.Message;
                if (!(ex is BusiException))
                    Logger.Error(ex.Message, ex);
                record.Status = RecordStatus.Fail;
            }
            finally
            {
                record.CompleteTime = Clock.Now;
                var repository = CurrentIocManager.Resolve<IJobContract>();
                await repository.AddRecordAsync(record);
            }
        }
    }
}
