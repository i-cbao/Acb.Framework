using Acb.AutoMapper;
using Acb.Core;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Contracts.Enums;
using Acb.Spear.ViewModels.Jobs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Controllers
{
    /// <summary> 任务控制器 </summary>
    [Route("api/job")]
    public class JobController : DController
    {
        private readonly ISchedulerContract _scheduler;
        private readonly IJobContract _contract;

        public JobController(ISchedulerContract scheduler, IJobContract contract)
        {
            _scheduler = scheduler;
            _contract = contract;
        }

        /// <summary> 添加任务 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<DResult> AddJob([FromBody]VJobInput input)
        {
            var inputDto = input.MapTo<JobInputDto>();
            inputDto.ProjectId = ProjectId;
            var dto = await _contract.CreateAsync(inputDto);
            await _scheduler.AddJob(dto);
            return DResult.Success;
        }

        /// <summary> 修改任务 </summary>
        /// <param name="jobId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{jobId:guid}")]
        public async Task<DResult> ModifyJob(Guid jobId, [FromBody]JobInputDto dto)
        {
            var result = await _contract.UpdateAsync(jobId, dto);
            return FromResult(result, "修改任务失败");
        }

        /// <summary> 获取所有任务 </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<DResults<VJob>> JobList(string keyword = null, JobStatus? status = null, int page = 1, int size = 10)
        {
            var pagedList = await _contract.PagedListAsync(new JobPagedInputDto
            {
                ProjectId = ProjectId,
                Keyword = keyword,
                Status = status,
                Page = page,
                Size = size
            });
            //Get Next Time
            await _scheduler.FillJobsTime(pagedList.List);
            return DResult.Succ(pagedList.MapPagedList<VJob, JobDto>());
        }

        /// <summary>
        /// 获取所有Job信息（简要信息 - 刷新数据的时候使用）
        /// </summary>
        /// <returns></returns>
        [HttpGet("hots")]
        public async Task<DResults<JobDto>> JobHots()
        {
            //:todo
            var list = await Task.FromResult(new List<JobDto>());
            return DResult.Succ(list, -1);
        }

        /// <summary> 暂停任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPut("pause/{jobId:guid}")]
        public async Task<DResult> PauseJob(Guid jobId)
        {
            var result = await _contract.UpdateStatusAsync(jobId, JobStatus.Disabled);
            if (result > 0)
                await _scheduler.PauseJob(jobId);
            return FromResult(result);
        }

        /// <summary> 恢复运行暂停的任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPut("resume/{jobId:guid}")]
        public async Task<DResult> ResumeJob(Guid jobId)
        {
            var code = await _contract.UpdateStatusAsync(jobId, JobStatus.Enabled);
            if (code > 0)
            {
                await _scheduler.ResumeJob(jobId);
            }
            return FromResult(code);
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete("{jobId:guid}")]
        public async Task<DResult> RemoveJob(Guid jobId)
        {
            var result = await _contract.RemoveAsync(jobId);
            if (result > 0)
            {
                await _scheduler.RemoveJob(jobId);
            }

            return FromResult(result, "删除任务失败");
        }

        /// <summary> 查询任务 </summary>
        /// <returns></returns>
        [HttpGet("{jobId:guid}")]
        public async Task<DResult<JobDto>> QueryJob(Guid jobId)
        {
            var dto = await _contract.GetAsync(jobId);
            return DResult.Succ(dto);
        }

        /// <summary> 立即执行 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost("start/{jobId:guid}")]
        public async Task<DResult> TriggerJob(Guid jobId)
        {
            await _scheduler.TriggerJob(jobId);
            return Success;
        }

        /// <summary> 获取任务日志 </summary>
        /// <param name="jobId"></param>
        /// <param name="triggerId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("logs/{jobId:guid}")]
        public async Task<DResults<VJobRecord>> GetJobLogs(Guid jobId, Guid? triggerId = null, int page = 1, int size = 10)
        {
            var list = await _contract.RecordsAsync(jobId, triggerId, page, size);
            return DResult.Succ(list.MapPagedList<VJobRecord, JobRecordDto>());
        }

        #region 触发器
        /// <summary> 获取任务触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("triggers/{jobId:guid}")]
        public async Task<DResults<VTrigger>> GetTriggers(Guid jobId)
        {
            var list = await _contract.GetTriggersAsync(jobId);
            await _scheduler.FillTriggersTime(list);
            return Succ(list.MapTo<VTrigger>(), -1);
        }

        /// <summary> 获取任务触发器 </summary>
        /// <param name="jobId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("trigger/{jobId:guid}")]
        public async Task<DResult> CreateTrigger(Guid jobId, [FromBody]VTriggerInput input)
        {
            var dto = input.MapTo<TriggerInputDto>();
            var result = await _contract.CreateTriggerAsync(jobId, dto);
            return FromResult(result, "创建触发器失败");
        }

        /// <summary> 更新任务触发器 </summary>
        /// <param name="triggerId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("trigger/{triggerId:guid}")]
        public async Task<DResult> UpdateTrigger(Guid triggerId, [FromBody]VTriggerInput input)
        {
            var dto = input.MapTo<TriggerInputDto>();
            var result = await _contract.UpdateTriggerAsync(triggerId, dto);
            if (result > 0)
            {
                await _scheduler.ResetTrigger(triggerId);
            }
            return FromResult(result, "更新触发器失败");
        }

        /// <summary> 更新触发器状态 </summary>
        /// <param name="triggerId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("trigger/{triggerId:guid}/status")]
        public async Task<DResult> UpdateTriggerStatus(Guid triggerId, TriggerStatus status)
        {
            var result = await _contract.UpdateTriggerStatusAsync(triggerId, status);
            if (result > 0)
            {
                switch (status)
                {
                    case TriggerStatus.Disable:
                        await _scheduler.PauseTrigger(triggerId);
                        break;
                    case TriggerStatus.Enable:
                        await _scheduler.ResumeTrigger(triggerId);
                        break;
                }
            }
            return FromResult(result, "更新触发器状态失败");
        }

        /// <summary> 删除触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        [HttpDelete("trigger/{triggerId:guid}")]
        public async Task<DResult> RemoveTriggers(Guid triggerId)
        {
            var result = await _contract.UpdateTriggerStatusAsync(triggerId, TriggerStatus.Delete);
            if (result > 0)
            {
                await _scheduler.PauseTrigger(triggerId);
            }
            return FromResult(result);
        } 
        #endregion

        /// <summary> 启动调度 </summary>
        /// <returns></returns>
        [HttpPost("start")]
        public async Task<DResult> StartSchedule()
        {
            await _scheduler.Start();
            return Success;
        }

        /// <summary> 停止调度 </summary>
        [HttpPost("stop")]
        public async Task<DResult> StopSchedule()
        {
            await _scheduler.Stop();
            return Success;
        }

        /// <summary> 是否在运行 </summary>
        [HttpGet("status")]
        public DResult Status()
        {
            return FromResult(_scheduler.IsRunning, "任务调度中心已停止");
        }
    }
}
