using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Domain;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Contracts.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Acb.Spear.Business.Domain.Repositories
{
    /// <summary> 任务仓储 </summary>
    public class JobRepository : DapperRepository<TJob>
    {
        /// <summary> 查询所有任务 </summary>
        /// <returns></returns>
        public async Task<PagedList<TJob>> QueryPagedAsync(Guid? projectId = null, string keyword = null,
            JobStatus? status = null, int page = 1, int size = 10)
        {
            SQL sql = "SELECT * FROM [t_job] WHERE 1=1";
            if (projectId.HasValue)
            {
                sql = (sql + "AND [ProjectId]=@projectId")["projectId", projectId];
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql = (sql + "AND ([Name] LIKE @keywork OR [Group] LIKE @keyword)")["keyword", $"%{keyword}%"];
            }

            if (status.HasValue)
            {
                sql = (sql + "AND [Status]=@status")["status", status];
            }
            else
            {
                sql = (sql + "AND [Status]<>@status")["status", JobStatus.Delete];
            }

            sql += "ORDER BY [Group],[CreationTime] DESC";
            var sqlStr = Connection.FormatSql(sql.ToString());
            return await Connection.PagedListAsync<TJob>(sqlStr, page, size, sql.Parameters());
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public Task<IEnumerable<TJobHttp>> QueryHttpDetailsAsync(IEnumerable<Guid> jobIds)
        {
            const string sql = "SELECT * FROM [t_job_http] WHERE [Id] = any(:jobIds)";
            var fmtSql = Connection.FormatSql(sql);
            return Connection.QueryAsync<TJobHttp>(fmtSql, new { jobIds = jobIds.ToArray() });
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<IDictionary<Guid, DateTime?>> QueryTimesAsync(IEnumerable<Guid> jobIds)
        {
            const string sql =
                "SELECT [JobId],MAX([StartTime]) as [PrevTime] FROM [t_job_record] WHERE [JobId] = any(:jobIds) GROUP BY [JobId]";
            var list = await Connection.QueryAsync<TJobTrigger>(Connection.FormatSql(sql),
                new { jobIds = jobIds.ToArray() });
            return list.ToDictionary(k => k.JobId, v => v.PrevTime);
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<TJobHttp> QueryHttpDetailByIdAsync(Guid jobId)
        {
            const string sql = "SELECT * FROM [t_job_http] WHERE [Id] = @jobId";
            var fmtSql = Connection.FormatSql(sql);
            return await Connection.QueryFirstOrDefaultAsync<TJobHttp>(fmtSql, new { jobId });
        }

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(JobDto dto)
        {
            var job = dto.MapTo<TJob>();
            return UnitOfWork.Trans(async () =>
            {
                var count = await Connection.InsertAsync(job, trans: Trans);
                switch (dto.Type)
                {
                    case JobType.Http:
                        var detail = dto.Detail.MapTo<TJobHttp>();
                        count += await Connection.InsertAsync(detail, trans: Trans);
                        break;
                }
                return count;
            });
        }

        /// <summary> 查询任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<JobDto> QueryByIdAsync(Guid jobId)
        {
            var dto = (await Connection.QueryByIdAsync<TJob>(jobId)).MapTo<JobDto>();
            switch (dto.Type)
            {
                case JobType.Http:
                    var model = await QueryHttpDetailByIdAsync(jobId);
                    dto.Detail = model.MapTo<HttpDetailDto>();
                    if (!string.IsNullOrWhiteSpace(model.Header))
                        dto.Detail.Header = JsonConvert.DeserializeObject<IDictionary<string, string>>(model.Header);
                    break;
            }
            return dto;
        }

        /// <summary> 更新任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(JobDto dto)
        {
            var job = dto.MapTo<TJob>();
            return UnitOfWork.Trans(async () =>
            {
                var count = await Connection.UpdateAsync(job,
                    new[]
                    {
                        nameof(TJob.Group), nameof(TJob.Name), nameof(TJob.Desc),
                        nameof(TJob.Type)
                    }, Trans);
                switch (dto.Type)
                {
                    case JobType.Http:
                        var detail = dto.Detail.MapTo<TJobHttp>();
                        count += await Connection.UpdateAsync(detail,
                            new[]
                            {
                                nameof(TJobHttp.Url), nameof(TJobHttp.Method), nameof(TJobHttp.Data),
                                nameof(TJobHttp.Header), nameof(TJobHttp.BodyType)
                            }, Trans);
                        break;
                }
                return count;
            });
        }

        /// <summary> 更新任务状态 </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task<int> UpdateStatusAsync(Guid jobId, JobStatus status)
        {
            return Connection.UpdateAsync(new TJob
            {
                Id = jobId,
                Status = (byte)status
            }, new[] { nameof(TJob.Status) }, Trans);
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public Task DeleteByIdAsync(Guid jobId)
        {
            UnitOfWork.Trans(() =>
            {
                Connection.Delete<TJob>(jobId, trans: Trans);
                Connection.Delete<TJobHttp>(jobId, trans: Trans);
                Connection.Delete<TJobTrigger>(jobId, "JobId", Trans);
                Connection.Delete<TJobRecord>(jobId, "JobId", Trans);
            });
            return Task.CompletedTask;
        }
    }
}
