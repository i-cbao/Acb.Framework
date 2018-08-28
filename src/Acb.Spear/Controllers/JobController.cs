using Acb.Core;
using Acb.Spear.Domain;
using Acb.Spear.Domain.Dtos;
using Acb.Spear.Domain.Enums;
using Acb.Spear.Scheduler;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Spear.Controllers
{
    /// <summary> 任务控制器 </summary>
    [Route("api/job")]
    public class JobController : Controller
    {
        private readonly SchedulerCenter _scheduler;
        private readonly JobRepository _repository;

        public JobController(SchedulerCenter scheduler, JobRepository repository)
        {
            _scheduler = scheduler;
            _repository = repository;
        }

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<DResult> AddJob([FromBody]JobDto dto)
        {
            await _repository.InsertJob(dto);
            await _scheduler.AddJob(dto);
            return DResult.Success;
        }

        /// <summary> 获取所有任务 </summary>
        [HttpGet("")]
        public async Task<DResults<JobDto>> JobList(string keyword = null, JobStatus status = JobStatus.All, int page = 1, int size = 10)
        {
            var pagedList = await _repository.QueryJobs(keyword, status, page, size);
            var triggers = pagedList.List.SelectMany(t => t.Triggers);
            await _scheduler.SchedulerTriggers(triggers);
            return DResult.Succ(pagedList);
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
        [HttpPost("pause/{jobId}")]
        public async Task<DResult> PauseJob(string jobId)
        {
            await _repository.UpdateStatus(jobId, JobStatus.Pause);
            return await _scheduler.StopJob(jobId);
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete("{jobId}")]
        public async Task<DResult> RemoveJob(string jobId)
        {
            await _repository.DeleteById(jobId);
            return await _scheduler.DeleteJob(jobId);
        }

        /// <summary> 恢复运行暂停的任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost("resume/{jobId}")]
        public async Task<DResult> ResumeJob(string jobId)
        {
            await _repository.UpdateStatus(jobId, JobStatus.Start);
            return await _scheduler.ResumeJob(jobId);
        }

        /// <summary> 查询任务 </summary>
        /// <returns></returns>
        [HttpGet("{jobId}")]
        public async Task<DResult<JobDto>> QueryJob(string jobId)
        {
            var dto = await _repository.QueryByJobId(jobId);
            await _scheduler.SchedulerTriggers(dto.Triggers);
            return DResult.Succ(dto);
        }

        /// <summary> 修改任务 </summary>
        /// <param name="jobId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{jobId}")]
        public async Task<DResult> ModifyJob(string jobId, [FromBody]JobDto dto)
        {
            await _repository.DeleteById(jobId);
            await _repository.InsertJob(dto);
            await _scheduler.DeleteJob(dto.Id);
            await _scheduler.AddJob(dto);
            return DResult.Success;
        }

        /// <summary> 立即执行 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost("start/{jobId}")]
        public async Task<DResult> TriggerJob(string jobId)
        {
            return await _scheduler.TriggerJob(jobId) ? DResult.Success : DResult.Error("执行失败");
        }

        /// <summary> 获取任务日志 </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("logs/{jobId}")]
        public async Task<DResults<JobRecordDto>> GetJobLogs(string jobId, int page = 1, int size = 10)
        {
            var list = await _repository.QueryRecords(jobId, page, size);
            return DResult.Succ(list);
        }

        /// <summary> 启动调度 </summary>
        /// <returns></returns>
        [HttpPost("start")]
        public async Task<DResult> StartSchedule()
        {
            return await _scheduler.Start() ? DResult.Success : DResult.Error("启动任务调度中心失败");
        }

        /// <summary> 停止调度 </summary>
        [HttpPost("stop")]
        public async Task<DResult> StopSchedule()
        {
            return await _scheduler.Stop() ? DResult.Success : DResult.Error("停止任务调度中心失败");
        }

        /// <summary> 是否在运行 </summary>
        [HttpGet("status")]
        public DResult Status()
        {
            return _scheduler.IsRunning ? DResult.Success : DResult.Error("任务调度中心已停止");
        }
    }
}
