using Acb.Core;
using Acb.Core.Domain;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Business
{
    public class JobService : DService, IJobContract
    {
        public Task<JobDto> CreateAsync(JobDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<JobDto>> PagedListAsync(string keyword = null, JobStatus status = JobStatus.All, int page = 1, int size = 10)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Guid jobId, JobStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<JobDto> GetAsync(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveAsync(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<HttpDetailDto> GetHttpDetailAsync(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<Guid, HttpDetailDto>> GetHttpDetailsAsync(List<Guid> jobIds)
        {
            throw new NotImplementedException();
        }

        public Task<List<TriggerDto>> GetTriggersAsync(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<Guid, List<TriggerDto>>> GetTriggersAsync(List<Guid> jobIds)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddRecordAsync(JobRecordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<JobRecordDto>> RecordsAsync(Guid jobId, int page = 1, int size = 10)
        {
            throw new NotImplementedException();
        }
    }
}
