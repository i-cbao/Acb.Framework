using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Adapters;
using Acb.Dapper.Domain;
using Acb.Middleware.JobScheduler.Domain.Dtos;
using Acb.Middleware.JobScheduler.Domain.Entities;
using Acb.Middleware.JobScheduler.Domain.Enums;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Middleware.JobScheduler.Domain
{
    /// <summary> 任务仓储 </summary>
    public class JobRepository : DapperRepository<TJob>
    {
        /// <summary> 查询所有任务 </summary>
        /// <returns></returns>
        public async Task<List<JobDto>> QueryJobs(string keyword = null, JobStatus status = JobStatus.All)
        {
            SQL sql = "SELECT * FROM [t_job] WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql = (sql + "AND ([Name] LIKE @keywork OR [Group] LIKE @keyword)")["keyword", $"%{keyword}%"];
            }

            if (status != JobStatus.All)
            {
                sql = (sql + "AND [Status]=@status")["status", status];
            }

            sql += "ORDER BY [CreationTime] DESC";
            using (var conn = GetConnection(threadCache: false))
            {
                var sqlStr = conn.FormatSql(sql.ToString());
                var jobs = (await conn.QueryAsync<JobDto>(sqlStr, sql.Parameters())).ToList();
                if (!jobs.Any()) return jobs;
                var ids = jobs.Select(t => t.Id).ToArray();
                var triggers = await QueryTriggers(ids);
                var https = await QueryHttpJobs(ids);
                foreach (var dto in jobs)
                {
                    if (https.ContainsKey(dto.Id))
                        dto.Detail = https[dto.Id];
                    if (triggers.ContainsKey(dto.Id))
                        dto.Triggers = triggers[dto.Id];
                }

                return jobs;
            }
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, HttpDetailDto>> QueryHttpJobs(IEnumerable<string> jobIds)
        {
            const string sql = "SELECT * FROM [t_job_http] WHERE [Id] = any(:jobIds)";
            using (var conn = GetConnection(threadCache: false))
            {
                var fmtSql = conn.FormatSql(sql);
                var list = await conn.QueryAsync<HttpDetailDto>(fmtSql, new { jobIds = jobIds.ToArray() });
                return list.ToDictionary(k => k.Id, v => v);
            }
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<TriggerDto>>> QueryTriggers(IEnumerable<string> jobIds)
        {
            const string sql = "SELECT * FROM [t_job_trigger] WHERE [JobId] = any(:jobIds)";
            using (var conn = GetConnection(threadCache: false))
            {
                var list = await conn.QueryAsync<TriggerDto>(conn.FormatSql(sql),
                    new { jobIds = jobIds.ToArray() });
                return list.GroupBy(t => t.JobId).ToDictionary(k => k.Key, v => v.ToList());
            }
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<HttpDetailDto> QueryHttpJobById(string jobId)
        {
            const string sql = "SELECT * FROM [t_job_http] WHERE [Id] = @jobId";
            using (var conn = GetConnection(threadCache: false))
            {
                var fmtSql = conn.FormatSql(sql);
                return await conn.QueryFirstOrDefaultAsync<HttpDetailDto>(fmtSql, new { jobId });
            }
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TriggerDto>> QueryTriggersById(string jobId)
        {
            const string sql = "SELECT * FROM [t_job_trigger] WHERE [JobId] = @jobId";
            using (var conn = GetConnection(threadCache: false))
            {
                var fmtSql = conn.FormatSql(sql);
                return await conn.QueryAsync<TriggerDto>(fmtSql, new { jobId });
            }
        }

        /// <summary> 更新触发器次数 </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task UpdateTriggerTimes(string id, int count = -1)
        {
            const string sql =
                "UPDATE [t_job_trigger] SET [Times]=[Times]+@count,[PrevTime]=now() WHERE [JobId] = @id AND [Type]=2 AND [Times]>0";
            using (var conn = GetConnection(threadCache: false))
            {
                var fmtSql = conn.FormatSql(sql);
                await conn.ExecuteAsync(fmtSql, new { id, count });
            }
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
        public async Task<JobDto> QueryByJobId(string jobId)
        {
            var dto = QueryById(jobId).MapTo<JobDto>();
            dto.Detail = await QueryHttpJobById(jobId);
            dto.Triggers = (await QueryTriggersById(jobId)).ToList();
            return dto;
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
        public async Task UpdateStatus(string jobId, JobStatus status)
        {
            using (var conn = GetConnection(threadCache: false))
            {
                await conn.UpdateAsync(new TJob
                {
                    Id = jobId,
                    Status = (int)status
                }, new[] { nameof(TJob.Status) });
            }
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task DeleteById(string jobId)
        {
            Transaction((conn, trans) =>
            {
                conn.Delete<TJob>(jobId);
                conn.Delete<TJobHttp>(jobId);
                conn.Delete<TJobTrigger>(jobId, "JobId");
                conn.Delete<TJobRecord>(jobId, "JobId");
            });
            return Task.CompletedTask;
        }

        /// <summary> 查询任务记录 </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<JobRecordDto>> QueryRecords(string jobId, int page = 1, int size = 20)
        {
            SQL sql = "SELECT * FROM [t_job_record] WHERE [JobId]=@jobId ORDER BY [StartTime] DESC";
            using (var conn = GetConnection(threadCache: false))
            {
                return await sql.PagedListAsync<JobRecordDto>(conn, page, size, new { jobId });
            }
        }

        /// <summary> 添加任务日志 </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task InsertRecord(TJobRecord record)
        {
            using (var conn = GetConnection(threadCache: false))
            {
                await conn.InsertAsync(record);
            }
        }
    }
}
