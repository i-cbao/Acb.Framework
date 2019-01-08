using Acb.Core;
using Acb.Core.Dependency;
using Acb.Spear.Contracts.Dtos;
using System;
using System.Threading.Tasks;
using Acb.Spear.Contracts.Dtos.Account;

namespace Acb.Spear.Contracts
{
    /// <summary> 账户相关契约 </summary>
    public interface IAccountContract : IDependency
    {
        /// <summary> 创建账户 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<AccountDto> CreateAsync(AccountInputDto inputDto);

        /// <summary> 账户登录 </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<AccountDto> LoginAsync(string account, string password);

        Task<int> UpdateAsync(Guid id, AccountInputDto inputDto);

        Task<PagedList<AccountRecordDto>> LoginRecordsAsync(Guid id, int page = 1, int size = 10);
    }
}
