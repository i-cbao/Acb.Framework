using Acb.Core.Dependency;
using Acb.Spear.Contracts.Dtos.Job;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Contracts
{
    public interface ISchedulerContract : ISingleDependency
    {
        /// <summary> 启动调度中心 </summary>
        Task Start();

        /// <summary> 停止调度中心 </summary>
        /// <returns></returns>
        Task Stop();

        /// <summary> 是否在运行 </summary>
        bool IsRunning { get; }
        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task AddJob(JobDto dto);

        /// <summary> 暂停任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task PauseJob(Guid jobId);

        /// <summary> 恢复任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task ResumeJob(Guid jobId);

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task RemoveJob(Guid jobId);

        /// <summary> 立即执行任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task TriggerJob(Guid jobId);

        /// <summary> 获取任务下次执行时间 </summary>
        /// <param name="jobDtos"></param>
        /// <returns></returns>
        Task FillJobsTime(IEnumerable<JobDto> jobDtos);

        /// <summary> 获取触发器的下次执行时间 </summary>
        /// <param name="triggerDtos"></param>
        /// <returns></returns>
        Task FillTriggersTime(IEnumerable<TriggerDto> triggerDtos);

        /// <summary> 重置触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        Task ResetTrigger(Guid triggerId);

        /// <summary> 暂停触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        Task PauseTrigger(Guid triggerId);

        /// <summary> 恢复触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        Task ResumeTrigger(Guid triggerId);
    }
}
