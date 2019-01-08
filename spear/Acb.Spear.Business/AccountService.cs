using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Spear.Business.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos.Account;
using Acb.Spear.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Business
{
    public class AccountService : DService, IAccountContract
    {
        private readonly AccountRepository _repository;
        private readonly AccounrRecordRepository _recordRepository;

        public AccountService(AccountRepository repository, AccounrRecordRepository recordRepository)
        {
            _repository = repository;
            _recordRepository = recordRepository;
        }

        public async Task<AccountDto> CreateAsync(AccountInputDto inputDto)
        {
            if (await _repository.ExistsAccountAsync(inputDto.Account))
                throw new BusiException("登录帐号已存在");
            var model = inputDto.MapTo<TAccount>();
            model.Id = IdentityHelper.NewSequentialGuid();
            model.PasswordSalt = IdentityHelper.Guid16;
            model.Password = $"{model.Password},{model.PasswordSalt}".Md5();
            model.CreateTime = Clock.Now;
            var result = await _repository.InsertAsync(model);
            if (result <= 0)
                throw new BusiException("创建账户失败");
            return model.MapTo<AccountDto>();
        }

        public async Task<AccountDto> LoginAsync(string account, string password)
        {
            var model = await _repository.QueryAccountAsync(account);
            if (model == null)
                throw new BusiException("登录帐号不存在");
            var record = new TAccountRecord
            {
                Id = IdentityHelper.NewSequentialGuid(),
                AccountId = model.Id,
                CreateTime = Clock.Now,
                CreateIp = AcbHttpContext.ClientIp,
                UserAgent = AcbHttpContext.UserAgent ?? "client"
            };
            if (!string.Equals($"{password},{model.PasswordSalt}".Md5(), model.Password))
            {
                record.Status = (short)AccountRecordStatus.Fail;
                record.Remark = "登录密码不正确";
            }
            else
            {
                record.Status = (short)AccountRecordStatus.Success;
                record.Remark = "登录成功";
            }

            await _recordRepository.InsertAsync(record);
            if (record.Status != (short)AccountRecordStatus.Success)
                throw new BusiException(record.Remark);
            return model.MapTo<AccountDto>();
        }

        public Task<int> UpdateAsync(Guid id, AccountInputDto inputDto)
        {
            var model = inputDto.MapTo<TAccount>();
            model.Id = id;
            return _repository.UpdateAsync(model);
        }

        public async Task<PagedList<AccountRecordDto>> LoginRecordsAsync(Guid id, int page = 1, int size = 10)
        {
            var paged = await _recordRepository.QueryPagedListAsync(id, page, size);
            return paged.MapPagedList<AccountRecordDto, TAccountRecord>();
        }
    }
}
