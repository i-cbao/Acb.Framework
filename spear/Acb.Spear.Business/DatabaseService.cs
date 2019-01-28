using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Spear.Business.Database;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Business
{
    public class DatabaseService : IDatabaseContract
    {
        private readonly DataBaseRepository _repository;

        public DatabaseService(DataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddAsync(Guid accountId, string name, ProviderType provider, string connectionString)
        {
            var model = new TDatabase
            {
                Id = IdentityHelper.NewSequentialGuid(),
                AccountId = accountId,
                Name = name,
                Provider = provider.ToString(),
                ConnectionString = connectionString,
                CreateTime = Clock.Now,
                Status = (byte)CommonStatus.Normal
            };
            return await _repository.InsertAsync(model);
        }

        public async Task<DatabaseTablesDto> GetAsync(Guid id)
        {
            var model = await _repository.QueryByIdAsync(id);
            var uw = new UnitOfWork(model.ConnectionString, model.Provider);
            var service = uw.Service();
            var tables = await service.GetTablesAsync();
            return new DatabaseTablesDto
            {
                Name = model.Name,
                DbName = service.DbName,
                Provider = service.Provider,
                Tables = tables
            };
        }

        public async Task<PagedList<DatabaseDto>> PagedListAsync(Guid accountId, ProviderType? type = null, int page = 1,
            int size = 10)
        {
            return await _repository.PagedListAsync(accountId, type, page, size);
        }

        public async Task<int> SetAsync(Guid id, string name, ProviderType type, string connectionString)
        {
            return await _repository.UpdateAsync(id, name, type, connectionString);
        }

        public async Task<int> RemoveAsync(Guid id)
        {
            return await _repository.UpdateStatusAsync(id, CommonStatus.Delete);
        }

        public string ConvertToLanguageType(string dbType, ProviderType provider, LanguageType language, bool isNullable = false)
        {
            return DbTypeConverter.Instance.Convert(provider, language, dbType, isNullable);
        }

        public string ConvertToDbType(string languageType, ProviderType provider, LanguageType language)
        {
            var dbType = DbTypeConverter.Instance.DbType(provider, language, languageType);
            if (languageType.EndsWith("?"))
                dbType += "?";
            return dbType;
        }
    }
}
