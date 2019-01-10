using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Business
{
    public class ConfigService : DService, IConfigContract
    {
        private readonly ConfigRepository _repository;

        public ConfigService(ConfigRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<string>> GetNamesAsync(Guid projectId)
        {
            return _repository.QueryNamesAsync(projectId);
        }

        public Task<IEnumerable<string>> GetEnvsAsync(Guid projectId, string module)
        {
            return _repository.QueryModesAsync(projectId, module);
        }

        public Task<string> GetAsync(Guid projectId, string module, string env = null)
        {
            return _repository.QueryByModuleAsync(projectId, module, env);
        }

        public async Task<ConfigDto> DetailAsync(Guid configId)
        {
            var model = await _repository.QueryByIdAsync(configId);
            return model.MapTo<ConfigDto>();
        }

        public Task<string> GetVersionAsync(Guid projectId, string module, string env = null)
        {
            return _repository.QueryVersionAsync(projectId, module, env);
        }

        public async Task<PagedList<ConfigDto>> GetHistoryAsync(Guid projectId, string module, string env = null, int page = 1, int size = 10)
        {
            var models = await _repository.QueryHistoryAsync(projectId, module, env, page, size);
            return models.MapPagedList<ConfigDto, TConfig>();
        }

        public async Task<ConfigDto> RecoveryAsync(Guid historyId)
        {
            var model = await _repository.RecoveryAsync(historyId);
            return model.MapTo<ConfigDto>();
        }

        public Task<int> SaveAsync(ConfigDto dto)
        {
            return _repository.UpdateAsync(dto.MapTo<TConfig>());
        }

        public Task<int> RemoveAsync(Guid projectId, string module, string env)
        {
            return _repository.DeleteByModuleAsync(projectId, module, env);
        }

        public Task<int> RemoveAsync(Guid configId)
        {
            return _repository.DeleteAsync(configId);
        }
    }
}
