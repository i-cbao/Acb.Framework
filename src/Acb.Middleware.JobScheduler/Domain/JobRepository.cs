using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Middleware.JobScheduler.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Entities;
using Acb.Middleware.JobScheduler.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Middleware.JobScheduler.Domain
{
    public class JobRepository : DapperRepository<TJob>
    {
        /// <summary> 查询所有任务 </summary>
        /// <returns></returns>
        public Task<List<JobDto>> QueryJobs(string keyword = null, JobStatus status = JobStatus.All)
        {
            //:todo
            return Task.FromResult(new List<JobDto>());
        }

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task InsertJob(JobDto dto)
        {
            dto.Id = dto.Detail.Id = IdentityHelper.Guid32;
            dto.CreationTime = Clock.Now;
            var job = dto.MapTo<TJob>();
            var triggers = new List<TJobTrigger>();
            foreach (var trigger in dto.Triggers)
            {
                trigger.Id = IdentityHelper.Guid32;
                trigger.JobId = dto.Id;
                triggers.Add(trigger.MapTo<TJobTrigger>());
            }

            Transaction((conn, trans) =>
            {
                conn.Insert(job);
                switch (dto.Type)
                {
                    case JobType.Http:
                        var detail = dto.Detail.MapTo<TJobHttp>();
                        conn.Insert(detail);
                        break;
                }

                conn.Insert<TJobTrigger>(triggers.ToArray());
            });
            return Task.CompletedTask;
        }

        /// <summary> 查询任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task<JobDto> QueryById(string jobId)
        {
            //:todo
            return Task.FromResult(new JobDto());
        }

        /// <summary> 更新任务 </summary>
        /// <param name="jobId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task Update(string jobId, JobDto dto)
        {
            //:todo
            return Task.CompletedTask;
        }

        /// <summary> 更新任务状态 </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task UpdateStatus(string jobId, JobStatus status)
        {
            return Connection.UpdateAsync(new TJob
            {
                Id = jobId,
                Status = (int)status
            }, new[] { nameof(TJob.Status) });
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task DeleteById(string jobId)
        {
            return Connection.DeleteAsync<TJob>(jobId);
        }

        /// <summary> 查询任务记录 </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Task<PagedList<JobRecordDto>> QueryRecords(string jobId, int page = 1, int size = 20)
        {
            SQL sql = "SELECT * FROM [t_job_record] WHERE [JobId]=@jobId ORDER BY [StartTime] DESC";
            return sql.PagedListAsync<JobRecordDto>(Connection, page, size, new { jobId });
        }

        /// <summary> 添加任务日志 </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public Task InsertRecord(TJobRecord record)
        {
            return Connection.InsertAsync(record);
        }
    }
}
