using Acb.Core;
using Acb.Middleware.JobScheduler.Domain;
using Acb.Middleware.JobScheduler.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Enums;
using Acb.Middleware.JobScheduler.Scheduler;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Middleware.JobScheduler.Controllers
{
    /// <summary> 任务控制器 </summary>
    [Route("api/job")]
    [ApiController]
    public class JobController : ControllerBase
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
        public async Task<List<JobDto>> JobList(string keyword = null, JobStatus status = JobStatus.All)
        {
            return await _repository.QueryJobs(keyword, status);
        }

        /// <summary>
        /// 获取所有Job信息（简要信息 - 刷新数据的时候使用）
        /// </summary>
        /// <returns></returns>
        [HttpGet("hots")]
        public async Task<List<JobDto>> JobHots()
        {
            //:todo
            return await Task.FromResult(new List<JobDto>());
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
        public async Task<JobDto> QueryJob(string jobId)
        {
            return await _repository.QueryById(jobId);
        }

        /// <summary> 修改 </summary>
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
        public async Task<bool> TriggerJob(string jobId)
        {
            await _scheduler.TriggerJob(jobId);
            return true;
        }

        /// <summary> 获取job日志 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet("logs/{jobId}")]
        public async Task<PagedList<JobRecordDto>> GetJobLogs(string jobId)
        {
            return await _repository.QueryRecords(jobId);
        }

        /// <summary> 启动调度 </summary>
        /// <returns></returns>
        [HttpPost("start")]
        public async Task<bool> StartSchedule()
        {
            return await _scheduler.Start();
        }

        /// <summary> 停止调度 </summary>
        [HttpPost("stop")]
        public async Task<bool> StopSchedule()
        {
            return await _scheduler.Stop();
        }


    }
}
