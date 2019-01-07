using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Acb.TimingTask
{
    /// <summary> 任务调度中心 </summary>
    public class SchedulerCenter
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private IScheduler _scheduler;
        private readonly IDictionary<string, string> _triggers;

        public SchedulerCenter(IServiceProvider provider, IDictionary<string, string> triggers = null)
        {
            _logger = LogManager.Logger<SchedulerCenter>();
            _provider = provider;
            _triggers = triggers ?? new Dictionary<string, string>();
        }

        private static ITrigger BuildTrigger(IJobDetail jobDetail, string corn)
        {
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity($"{jobDetail.Key.Name}_trigger");
            if (corn.IsMatch("^([^\\s]+\\s){5,6}[^\\s]+$"))
            {
                triggerBuilder.WithCronSchedule(corn);
            }
            else
            {
                var arr = corn.Split(',');
                var interval = arr[0].CastTo(0);
                var repeat = 0;
                if (arr.Length > 1)
                    repeat = arr[1].CastTo(0);
                triggerBuilder.WithSimpleSchedule(b =>
                {
                    b.WithIntervalInSeconds(interval);
                    if (repeat > 0)
                    {
                        b.WithRepeatCount(repeat);
                    }
                });
            }
            return triggerBuilder.ForJob(jobDetail).Build();
        }

        /// <summary> 开启任务调度 </summary>
        /// <returns></returns>
        public async Task StartScheduler()
        {
            var props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"}
            };
            var factory = new StdSchedulerFactory(props);
            _scheduler = await factory.GetScheduler();

            //jobs
            var jobs = _provider.GetServices<IJob>();
            foreach (var job in jobs)
            {
                var name = job.GetType().PropName();
                if (!_triggers.TryGetValue(name, out var corn))
                {
                    corn = $"jobs:trigger:{name}".Config<string>();
                }

                if (string.IsNullOrWhiteSpace(corn))
                    continue;
                var builder = JobBuilder.Create(job.GetType());
                var jobDetail = builder
                    .WithIdentity(name)
                    .Build();
                var trigger = BuildTrigger(jobDetail, corn);
                await _scheduler.ScheduleJob(jobDetail, trigger);
                _logger.Info($"job started -> {name},corn:{corn}");
            }
            await _scheduler.Start();
            _logger.Info("scheduler started");
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
            await _scheduler.Standby();
            _logger.Info("任务调度暂停！");
            return true;
        }

        /// <summary> 立即执行 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<bool> TriggerJob(string jobId)
        {
            var key = new JobKey(jobId);
            if (!await _scheduler.CheckExists(key))
                return false;
            await _scheduler.TriggerJob(key);
            return true;
        }

        /// <summary> 暂停 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task StopJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                if (!await _scheduler.CheckExists(key))
                {
                    throw new BusiException("任务不存在");
                }
                await _scheduler.PauseJob(key);
            }
            catch (Exception ex)
            {
                _logger.Error($"暂停任务失败！{ex.Message}", ex);
                throw new BusiException("暂停任务失败");
            }
        }

        /// <summary> 删除 指定的计划 </summary>
        /// <param name="jobId">任务Id</param>
        /// <returns></returns>
        public async Task DeleteJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                await _scheduler.DeleteJob(key);
            }
            catch (Exception ex)
            {
                _logger.Error($"删除任务失败！{ex.Message}", ex);
                throw new BusiException("删除任务失败");
            }
        }

        /// <summary> 恢复运行暂停的任务 </summary>
        /// <param name="jobId">任务Id</param>
        public async Task ResumeJob(string jobId)
        {
            try
            {
                var key = new JobKey(jobId);
                if (!await _scheduler.CheckExists(key))
                {
                    throw new BusiException("任务Id不存在");
                }
                //任务已经存在则暂停任务
                await _scheduler.ResumeJob(key);
                _logger.Info($"任务[{jobId}]恢复运行");
            }
            catch (Exception ex)
            {
                _logger.Error($"恢复任务失败！{ex.Message}", ex);
                throw new BusiException("恢复任务计划失败！");
            }
        }
    }
}
