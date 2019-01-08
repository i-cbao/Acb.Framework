using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Domain.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Domain
{
    /// <summary> 任务仓储 </summary>
    public class JobRepository : DapperRepository<TJob>
    {
        /// <summary> 查询所有任务 </summary>
        /// <returns></returns>
        public async Task<PagedList<JobDto>> QueryJobs(string keyword = null, JobStatus status = JobStatus.All, int page = 1, int size = 10)
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

            sql += "ORDER BY [Group],[CreationTime] DESC";
            var sqlStr = Connection.FormatSql(sql.ToString());
            var jobs = await Connection.PagedListAsync<JobDto>(sqlStr, page, size, sql.Parameters());
            if (jobs?.List == null || !jobs.List.Any())
                return jobs;
            var ids = jobs.List.Select(t => t.Id).ToArray();
            var triggers = await QueryTriggers(ids);
            var https = await QueryHttpJobs(ids);
            foreach (var dto in jobs.List)
            {
                if (https.ContainsKey(dto.Id))
                    dto.Detail = https[dto.Id];
                if (triggers.ContainsKey(dto.Id))
                    dto.Triggers = triggers[dto.Id];
            }

            return jobs;
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<Dictionary<Guid, HttpDetailDto>> QueryHttpJobs(IEnumerable<Guid> jobIds)
        {
            const string sql = "SELECT * FROM [t_job_http] WHERE [Id] = any(:jobIds)";
            var fmtSql = Connection.FormatSql(sql);
            var list = await Connection.QueryAsync<HttpDetailDto>(fmtSql, new { jobIds = jobIds.ToArray() });
            return list.ToDictionary(k => k.Id, v => v);
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<Dictionary<Guid, List<TriggerDto>>> QueryTriggers(IEnumerable<Guid> jobIds)
        {
            const string sql = "SELECT * FROM [t_job_trigger] WHERE [JobId] = any(:jobIds)";
            var list = await Connection.QueryAsync<TriggerDto>(Connection.FormatSql(sql),
                new { jobIds = jobIds.ToArray() });
            return list.GroupBy(t => t.JobId).ToDictionary(k => k.Key, v => v.ToList());
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<HttpDetailDto> QueryHttpJobById(Guid jobId)
        {
            const string sql = "SELECT * FROM [t_job_http] WHERE [Id] = @jobId";
            var fmtSql = Connection.FormatSql(sql);
            return await Connection.QueryFirstOrDefaultAsync<HttpDetailDto>(fmtSql, new { jobId });
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TriggerDto>> QueryTriggersById(Guid jobId)
        {
            const string sql = "SELECT * FROM [t_job_trigger] WHERE [JobId] = @jobId";
            var fmtSql = Connection.FormatSql(sql);
            return await Connection.QueryAsync<TriggerDto>(fmtSql, new { jobId });
        }

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task InsertJob(JobDto dto)
        {
            dto.Id = dto.Detail.Id = IdentityHelper.NewSequentialGuid();
            dto.CreationTime = Clock.Now;
            var job = dto.MapTo<TJob>();
            var triggers = new List<TJobTrigger>();
            foreach (var trigger in dto.Triggers)
            {
                trigger.Id = IdentityHelper.NewSequentialGuid();
                trigger.JobId = dto.Id;
                triggers.Add(trigger.MapTo<TJobTrigger>());
            }

            Transaction(() =>
            {
                Connection.Insert(job, trans: Trans);
                switch (dto.Type)
                {
                    case JobType.Http:
                        var detail = dto.Detail.MapTo<TJobHttp>();
                        Connection.Insert(detail, trans: Trans);
                        break;
                }

                Connection.Insert<TJobTrigger>(triggers.ToArray(), trans: Trans);
            });
            return Task.CompletedTask;
        }

        /// <summary> 查询任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<JobDto> QueryByJobId(Guid jobId)
        {
            var dto = (await Connection.QueryByIdAsync<TJob>(jobId)).MapTo<JobDto>();
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
        public async Task UpdateStatus(Guid jobId, JobStatus status)
        {
            await Connection.UpdateAsync(new TJob
            {
                Id = jobId,
                Status = (int)status
            }, new[] { nameof(TJob.Status) }, Trans);
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task DeleteById(Guid jobId)
        {
            Transaction(() =>
            {
                Connection.Delete<TJob>(jobId, trans: Trans);
                Connection.Delete<TJobHttp>(jobId, trans: Trans);
                Connection.Delete<TJobTrigger>(jobId, "JobId", Trans);
                Connection.Delete<TJobRecord>(jobId, "JobId", Trans);
            });
            return Task.CompletedTask;
        }

        /// <summary> 查询任务记录 </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<JobRecordDto>> QueryRecords(Guid jobId, int page = 1, int size = 20)
        {
            SQL sql = "SELECT * FROM [t_job_record] WHERE [JobId]=@jobId ORDER BY [StartTime] DESC";
            return await sql.PagedListAsync<JobRecordDto>(Connection, page, size, new { jobId });
        }

        /// <summary> 添加任务日志 </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task InsertRecord(TJobRecord record)
        {
            const string sql =
                "UPDATE [t_job_trigger] SET [PrevTime]=@start WHERE [JobId] = @id;" +
                "UPDATE [t_job_trigger] SET [Times]=[Times]-1 WHERE [JobId] = @id AND [Type]=2 AND [Times]>0;";
            var fmtSql = Connection.FormatSql(sql);
            await Connection.ExecuteAsync(fmtSql, new { id = record.JobId, start = record.StartTime }, Trans);
            await Connection.InsertAsync(record, trans: Trans);
        }


    }
}
