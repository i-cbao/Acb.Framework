using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Job;
using Acb.Spear.Contracts.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Spear.Business
{
    public class JobService : DService, IJobContract
    {
        private readonly JobRepository _repository;
        private readonly JobRecordRepository _recordRepository;
        private readonly JobTriggerRepository _triggerRepository;

        public JobService(JobRepository repository, JobRecordRepository recordRepository, JobTriggerRepository triggerRepository)
        {
            _repository = repository;
            _recordRepository = recordRepository;
            _triggerRepository = triggerRepository;
        }

        public async Task<JobDto> CreateAsync(JobInputDto inputDto)
        {
            var dto = inputDto.MapTo<JobDto>();
            dto.Id = dto.Detail.Id = IdentityHelper.NewSequentialGuid();
            dto.CreationTime = Clock.Now;
            dto.Status = JobStatus.Disabled;
            var result = await _repository.InsertAsync(dto);
            return result > 0 ? dto : null;
        }

        public async Task<int> UpdateAsync(Guid id, JobInputDto inputDto)
        {
            var dto = inputDto.MapTo<JobDto>();
            dto.Id = id;
            var result = await _repository.UpdateAsync(dto);
            return result;
        }

        public async Task<PagedList<JobDto>> PagedListAsync(JobPagedInputDto inputDto)
        {
            var models = await _repository.QueryPagedAsync(inputDto.ProjectId, inputDto.Keyword, inputDto.Status, inputDto.Page,
                inputDto.Size);
            var jobs = models.MapPagedList<JobDto, TJob>();
            if (jobs?.List == null || !jobs.List.Any())
                return jobs;
            var ids = jobs.List.Select(t => t.Id).ToList();
            var https = await GetHttpDetailsAsync(ids);
            var times = await _repository.QueryTimesAsync(ids);
            foreach (var dto in jobs.List)
            {
                if (https.ContainsKey(dto.Id))
                    dto.Detail = https[dto.Id].MapTo<HttpDetailDto>();
                if (!times.ContainsKey(dto.Id))
                    continue;
                dto.PrevTime = times[dto.Id];
            }

            return jobs;
        }

        public Task<int> UpdateStatusAsync(Guid jobId, JobStatus status)
        {
            return _repository.UpdateStatusAsync(jobId, status);
        }

        public Task<JobDto> GetAsync(Guid jobId)
        {
            return _repository.QueryByIdAsync(jobId);
        }

        public Task<int> RemoveAsync(Guid jobId)
        {
            return _repository.UpdateStatusAsync(jobId, JobStatus.Delete);
        }

        public async Task<HttpDetailDto> GetHttpDetailAsync(Guid jobId)
        {
            var model = await _repository.QueryHttpDetailByIdAsync(jobId);
            var dto = model.MapTo<HttpDetailDto>();
            if (!string.IsNullOrWhiteSpace(model.Header))
                dto.Header = JsonConvert.DeserializeObject<IDictionary<string, string>>(model.Header);
            return dto;
        }

        public async Task<IDictionary<Guid, HttpDetailDto>> GetHttpDetailsAsync(List<Guid> jobIds)
        {
            jobIds = jobIds.Distinct().ToList();
            var models = (await _repository.QueryHttpDetailsAsync(jobIds)).ToList();
            var dtos = new Dictionary<Guid, HttpDetailDto>();
            foreach (var model in models)
            {
                var dto = model.MapTo<HttpDetailDto>();
                if (!string.IsNullOrWhiteSpace(model.Header))
                    dto.Header = JsonConvert.DeserializeObject<IDictionary<string, string>>(model.Header);
                dtos.Add(model.Id, dto);
            }

            return dtos;
        }

        public async Task<List<TriggerDto>> GetTriggersAsync(Guid jobId)
        {
            var dto = await _triggerRepository.QueryByJobIdAsync(jobId);
            return (dto ?? new List<TriggerDto>()).ToList();
        }

        public async Task<TriggerDto> GetTriggerAsync(Guid triggerId)
        {
            var model = await _triggerRepository.QueryByIdAsync(triggerId);
            return model.MapTo<TriggerDto>();
        }

        public Task<int> CreateTriggerAsync(Guid jobId, TriggerInputDto inputDto)
        {
            var model = inputDto.MapTo<TJobTrigger>();
            model.Id = IdentityHelper.NewSequentialGuid();
            model.Status = (byte)TriggerStatus.Disable;
            model.JobId = jobId;
            model.CreateTime = Clock.Now;
            return _triggerRepository.InsertAsync(model);
        }

        public Task<int> UpdateTriggerAsync(Guid triggerId, TriggerInputDto inputDto)
        {
            var model = inputDto.MapTo<TJobTrigger>();
            model.Id = triggerId;
            return _triggerRepository.UpdateAsync(model);
        }

        public Task<int> UpdateTriggerStatusAsync(Guid triggerId, TriggerStatus status)
        {
            return _triggerRepository.UpdateStatusAsync(triggerId, status);
        }

        public Task<IDictionary<Guid, List<TriggerDto>>> GetTriggersAsync(List<Guid> jobIds)
        {
            return _triggerRepository.QueryByJobIdsAsync(jobIds);
        }

        public Task<int> AddRecordAsync(JobRecordDto dto)
        {
            var model = dto.MapTo<TJobRecord>();
            return _recordRepository.InsertAsync(model);
        }

        public Task<PagedList<JobRecordDto>> RecordsAsync(Guid jobId, Guid? triggerId = null, int page = 1, int size = 10)
        {
            return _recordRepository.QueryPagedByJobIdAsync(jobId, triggerId, page, size);
        }
    }
}
