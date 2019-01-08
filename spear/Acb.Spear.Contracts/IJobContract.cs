using Acb.Core;
using Acb.Core.Dependency;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Contracts
{
    /// <inheritdoc />
    /// <summary> 任务相关契约 </summary>
    public interface IJobContract : IDependency
    {
        /// <summary> 创建任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<JobDto> CreateAsync(JobDto dto);

        /// <summary> 分页列表 </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<JobDto>> PagedListAsync(string keyword = null, JobStatus status = JobStatus.All, int page = 1, int size = 10);

        /// <summary> 更新任务状态 </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(Guid jobId, JobStatus status);

        /// <summary> 获取任务信息 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<JobDto> GetAsync(Guid jobId);

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(Guid jobId);

        /// <summary> 获取Http任务详情 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<HttpDetailDto> GetHttpDetailAsync(Guid jobId);

        /// <summary> 批量获取Http任务详情 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        Task<IDictionary<Guid, HttpDetailDto>> GetHttpDetailsAsync(List<Guid> jobIds);

        /// <summary> 获取任务触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<List<TriggerDto>> GetTriggersAsync(Guid jobId);

        /// <summary> 批量获取任务触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        Task<IDictionary<Guid, List<TriggerDto>>> GetTriggersAsync(List<Guid> jobIds);



        /// <summary> 添加任务记录 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AddRecordAsync(JobRecordDto dto);

        /// <summary> 任务记录列表 </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<JobRecordDto>> RecordsAsync(Guid jobId, int page = 1, int size = 10);
    }
}
