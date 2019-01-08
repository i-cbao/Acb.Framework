using Acb.Core.Dependency;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Timing;
using Acb.Spear.Domain;
using Quartz;
using System;
using System.Threading.Tasks;
using Acb.Spear.Business.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos;
using Acb.Spear.Contracts.Dtos.Job;

namespace Acb.Spear.Scheduler
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
                Id = IdentityHelper.NewSequentialGuid(),
                StartTime = Clock.Now
            };
            try
            {
                var data = context.JobDetail.JobDataMap.Get(Constants.JobData).CastTo<T>();
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
                var repository = CurrentIocManager.Resolve<JobRepository>();
                await repository.InsertRecord(record);
            }
        }
    }
}
