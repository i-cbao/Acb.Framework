using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acb.Core.Data;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Contracts.Enums;
using Dapper;

namespace Acb.Spear.Business.Domain.Repositories
{
    public class JobTriggerRepository : DapperRepository<TJobTrigger>
    {
        /// <summary> 查询触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TriggerDto>> QueryByJobIdAsync(Guid jobId)
        {
            const string sql = "SELECT * FROM [t_job_trigger] WHERE [JobId] = @jobId AND [Status] <> 4 ORDER BY [CreateTime]";
            var fmtSql = Connection.FormatSql(sql);
            return await Connection.QueryAsync<TriggerDto>(fmtSql, new { jobId });
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<IDictionary<Guid, List<TriggerDto>>> QueryByJobIdsAsync(IEnumerable<Guid> jobIds)
        {
            const string sql = "SELECT * FROM [t_job_trigger] WHERE [JobId] = any(:jobIds) AND [Status] <> 4";
            var list = await Connection.QueryAsync<TriggerDto>(Connection.FormatSql(sql),
                new { jobIds = jobIds.ToArray() });
            return list.GroupBy(t => t.JobId).ToDictionary(k => k.Key, v => v.ToList());
        }

        public Task<int> UpdateAsync(TJobTrigger model)
        {
            return Connection.UpdateAsync(model,
                new[]
                {
                    nameof(TJobTrigger.Type), nameof(TJobTrigger.Start), nameof(TJobTrigger.Expired),
                    nameof(TJobTrigger.Corn), nameof(TJobTrigger.Times), nameof(TJobTrigger.Interval)
                }, Trans);
        }

        public Task<int> UpdateStatusAsync(Guid triggerId, TriggerStatus status)
        {
            return Connection.UpdateAsync(new TJobTrigger
            {
                Id = triggerId,
                Status = (byte)status
            }, new[] { nameof(TJobTrigger.Status) }, Trans);
        }
    }
}
