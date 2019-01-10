﻿using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Business
{
    public class ProjectService : DService, IProjectContract
    {
        private readonly ProjectRepository _repository;

        public ProjectService(ProjectRepository repository)
        {
            _repository = repository;
        }

        public Task<int> AddAsync(ProjectDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(ProjectDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDto> DetailByCodeAsync(string code)
        {
            return _repository.QueryByCodeAsync(code);
        }

        public async Task<ProjectDto> DetailAsync(Guid id)
        {
            var model = await _repository.QueryByIdAsync(id);
            return model.MapTo<ProjectDto>();
        }

        public Task<PagedList<ProjectDto>> PagedListAsync(int page = 1, int size = 10)
        {
            throw new NotImplementedException();
        }
    }
}
