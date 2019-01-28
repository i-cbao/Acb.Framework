using Acb.Core;
using Acb.Core.Data;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos.Job;
using Dapper;
using System;
using System.Threading.Tasks;
using Acb.Core.Domain;

namespace Acb.Spear.Business.Domain.Repositories
{
    public class JobRecordRepository : DapperRepository<TJobRecord>
    {
        /// <summary> 查询任务记录 </summary>
        /// <param name="jobId"></param>
        /// <param name="triggerId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<JobRecordDto>> QueryPagedByJobIdAsync(Guid jobId, Guid? triggerId, int page, int size)
        {
            SQL sql = "SELECT * FROM [t_job_record] WHERE [JobId]=@jobId";
            if (triggerId.HasValue)
            {
                sql += "AND [TriggerId]=@triggerId";
            }

            sql += "ORDER BY [StartTime] DESC";
            return await sql.PagedListAsync<JobRecordDto>(Connection, page, size, new { jobId, triggerId });
        }

        /// <summary> 添加任务日志 </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(TJobRecord record)
        {
            const string sql =
                "UPDATE [t_job_trigger] SET [PrevTime]=@start WHERE [Id] = @id;" +
                "UPDATE [t_job_trigger] SET [Times]=[Times]-1 WHERE [Id] = @id AND [Type]=2 AND [Times]>0;";
            var fmtSql = Connection.FormatSql(sql);
            return UnitOfWork.Trans(async () =>
            {
                var count = await Connection.ExecuteAsync(fmtSql, new { id = record.TriggerId, start = record.StartTime },
                    Trans);
                count += await Connection.InsertAsync(record, trans: Trans);
                return count;
            });
        }
    }
}
