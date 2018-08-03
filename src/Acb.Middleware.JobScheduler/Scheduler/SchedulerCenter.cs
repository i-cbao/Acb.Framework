using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Logging;
using Acb.Middleware.JobScheduler.Domain;
using Acb.Middleware.JobScheduler.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Enums;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Acb.Middleware.JobScheduler.Scheduler
{
    public class SchedulerCenter
    {
        private IScheduler _scheduler;
        private readonly ILogger _logger;

        public SchedulerCenter()
        {
            _logger = LogManager.Logger<SchedulerCenter>();
            StartScheduler().GetAwaiter().GetResult();
        }

        private async Task StartScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);
            _scheduler = await factory.GetScheduler();
            var jobs = await CurrentIocManager.Resolve<JobRepository>().QueryJobs();
            foreach (var dto in jobs)
            {
                if (dto.Status != JobStatus.Start)
                    continue;
                switch ((JobType)dto.Type)
                {
                    case JobType.Http:
                        await StartHttpJob(dto);
                        break;
                }
            }
            await _scheduler.Start();
        }

        private async Task StartHttpJob(JobDto dto)
        {
            var builder = JobBuilder.Create<JobHttp>();
            var map = new JobDataMap { { Constant.JobData, dto.Detail } };
            var jobDetail = builder
                .WithIdentity(dto.Id)
                .SetJobData(map)
                .Build();
            var triggers = new List<ITrigger>();
            foreach (var trigger in dto.Triggers)
            {
                var triggerBuilder = GetTrigger(trigger);
                triggerBuilder.ForJob(jobDetail);

                triggers.Add(triggerBuilder.Build());
            }

            await _scheduler.ScheduleJob(jobDetail, triggers.AsReadOnly(), true);
        }

        private static TriggerBuilder GetTrigger(TriggerDto trigger)
        {
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(trigger.Id);
            if (trigger.Start.HasValue)
                triggerBuilder.StartAt(trigger.Start.Value);
            if (trigger.Expired.HasValue)
                triggerBuilder.EndAt(trigger.Expired.Value);
            switch (trigger.Type)
            {
                case TriggerType.Cron:
                    triggerBuilder.WithCronSchedule(trigger.Corn);
                    break;
                case TriggerType.Simple:
                    triggerBuilder
                        .WithSimpleSchedule(b =>
                        {
                            var sb = b.WithIntervalInSeconds(trigger.Interval);
                            if (trigger.Times > 0)
                                sb.WithRepeatCount(trigger.Times);
                            else
                                sb.RepeatForever();
                        });
                    break;
            }

            return triggerBuilder;
        }

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddJob(JobDto dto)
        {
            switch (dto.Type)
            {
                case JobType.Http:
                    await StartHttpJob(dto);
                    break;
            }
        }

        /// <summary> 添加触发器 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddTrigger(TriggerDto dto)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(dto.JobId));
            if (job == null)
                return;
            var triggerBuilder = GetTrigger(dto);
            triggerBuilder.ForJob(job);
            await _scheduler.ScheduleJob(job, triggerBuilder.Build());
        }

        /// <summary> 开启调度器 </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            //开启调度器
            if (!_scheduler.InStandbyMode)
                return _scheduler.InStandbyMode;
            await _scheduler.Start();
            _logger.Info("任务调度启动！");
            return _scheduler.InStandbyMode;
        }

        /// <summary> 停止任务调度 </summary>
        public async Task<bool> Stop()
        {
            //判断调度是否已经关闭
            if (_scheduler.InStandbyMode)
                return !_scheduler.InStandbyMode;
            //等待任务运行完成
            await _scheduler.Standby(); //TODO  注意：Shutdown后Start会报错，所以这里使用暂停。
            _logger.Info("任务调度暂停！");
            return !_scheduler.InStandbyMode;
        }

        /// <summary> 立即执行 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<bool> TriggerJob(string jobId)
        {
            await _scheduler.TriggerJob(new JobKey(jobId));
            return true;
        }

        /// <summary> 暂停 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task<DResult> StopJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                await _scheduler.PauseJob(key);
                return DResult.Success;
            }
            catch (Exception ex)
            {
                _logger.Error($"暂停任务失败！{ex.Message}", ex);
                return DResult.Error(ex.Message);
            }
        }

        /// <summary> 删除 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task<DResult> DeleteJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                await _scheduler.DeleteJob(key);
                return DResult.Success;
            }
            catch (Exception ex)
            {
                _logger.Error($"删除任务失败！{ex.Message}", ex);
                return DResult.Error(ex.Message);
            }
        }

        /// <summary> 恢复运行暂停的任务 </summary>
        /// <param name="jobId">任务Id</param>
        public async Task<DResult> ResumeJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                if (!await _scheduler.CheckExists(key))
                    return DResult.Error("任务不存在");
                //任务已经存在则暂停任务
                await _scheduler.ResumeJob(key);
                _logger.Info($"任务[{jobId}]恢复运行");
                return DResult.Success;
            }
            catch (Exception ex)
            {
                _logger.Error($"恢复任务失败！{ex.Message}", ex);
                return DResult.Error("恢复任务计划失败！");
            }
        }
    }
}
