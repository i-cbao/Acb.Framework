using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Logging;
using Acb.Spear.Domain;
using Acb.Spear.Domain.Enums;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts.Dtos;
using Acb.Spear.Contracts.Dtos.Job;

namespace Acb.Spear.Scheduler
{
    public class SchedulerCenter
    {
        private IScheduler _scheduler;
        private readonly ILogger _logger;

        public SchedulerCenter()
        {
            _logger = LogManager.Logger<SchedulerCenter>();
        }

        public async Task StartScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);
            _scheduler = await factory.GetScheduler();
            var jobs = await CurrentIocManager.Resolve<JobRepository>().QueryJobs(size: 1000);
            foreach (var dto in jobs.List)
            {
                await RunJob(dto);
            }
            await _scheduler.Start();
        }

        private async Task RunJob(JobDto dto)
        {
            if (dto.Status != JobStatus.Start)
                return;
            switch (dto.Type)
            {
                case JobType.Http:
                    await StartHttpJob(dto);
                    break;
            }
        }

        private async Task StartHttpJob(JobDto dto)
        {
            var builder = JobBuilder.Create<JobHttp>();
            var map = new JobDataMap { { Constants.JobData, dto.Detail } };
            var jobDetail = builder
                .WithIdentity(dto.Id.ToString("N"))
                .SetJobData(map)
                .Build();
            var triggers = new List<ITrigger>();
            foreach (var trigger in dto.Triggers)
            {
                var triggerBuilder = GetTrigger(trigger);
                if (triggerBuilder == null)
                    continue;
                triggerBuilder.ForJob(jobDetail);

                triggers.Add(triggerBuilder.Build());
            }

            if (triggers.Any())
                await _scheduler.ScheduleJob(jobDetail, triggers.AsReadOnly(), true);
        }

        private static TriggerBuilder GetTrigger(TriggerDto trigger)
        {
            if (trigger.Type == TriggerType.Simple && trigger.Times == 0)
                return null;
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(trigger.Id.ToString("N"));
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
            await RunJob(dto);
        }

        /// <summary> 是否在运行 </summary>
        public bool IsRunning => _scheduler.IsStarted && !_scheduler.InStandbyMode;

        /// <summary> 开启调度器 </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            //开启调度器
            if (!_scheduler.InStandbyMode)
                return false;
            await _scheduler.Start();
            _logger.Info("任务调度启动！");
            return true;
        }

        /// <summary> 停止任务调度 </summary>
        public async Task<bool> Stop()
        {
            //判断调度是否已经关闭
            if (_scheduler.InStandbyMode)
                return false;
            //等待任务运行完成
            await _scheduler.Standby(); //TODO  注意：Shutdown后Start会报错，所以这里使用暂停。
            _logger.Info("任务调度暂停！");
            return true;
        }

        /// <summary> 立即执行 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<bool> TriggerJob(Guid jobId)
        {
            var key = new JobKey(jobId.ToString("N"));
            if (!await _scheduler.CheckExists(key))
                return false;
            await _scheduler.TriggerJob(key);
            return true;
        }

        /// <summary> 暂停 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task<DResult> StopJob(Guid jobId)
        {
            try
            {
                var key = new JobKey(jobId.ToString("N"));
                if (!await _scheduler.CheckExists(key))
                {
                    return DResult.Error("任务不存在");
                }
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
        public async Task<DResult> DeleteJob(Guid jobId)
        {
            try
            {
                var key = new JobKey(jobId.ToString("N"));
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
        public async Task<DResult> ResumeJob(Guid jobId)
        {
            try
            {
                var key = new JobKey(jobId.ToString("N"));
                if (!await _scheduler.CheckExists(key))
                {
                    var repository = CurrentIocManager.Resolve<JobRepository>();
                    var dto = await repository.QueryByJobId(jobId);
                    await RunJob(dto);
                    return DResult.Success;
                }
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

        /// <summary> 获取触发器执行时间 </summary>
        /// <returns></returns>
        public async Task SchedulerTriggers(IEnumerable<TriggerDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var trigger = await _scheduler.GetTrigger(new TriggerKey(dto.Id.ToString("N")));
                if (trigger == null)
                    continue;
                var prevTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime;
                if (prevTime.HasValue)
                    dto.PrevTime = prevTime;
                dto.NextTime = trigger.GetNextFireTimeUtc()?.LocalDateTime;
            }
        }
    }
}
