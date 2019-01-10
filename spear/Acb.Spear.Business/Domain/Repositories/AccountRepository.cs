using System.Threading.Tasks;
using Acb.AutoMapper;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos.Account;

namespace Acb.Spear.Business.Domain.Repositories
{
    public class AccountRepository : DapperRepository<TAccount>
    {
        public async Task<bool> ExistsAccountAsync(string account)
        {
            return await Connection.ExistsWhereAsync<TAccount>("[Account]=@account", new { account });
        }

        /// <summary> 查询账号 </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<TAccount> QueryAccountAsync(string account)
        {
            return await Connection.QueryByIdAsync<TAccount>(account, nameof(TAccount.Account));
        }

        public async Task<AccountDto> LoginAsync(string account, string password)
        {
            var model = await QueryAccountAsync(account);
            if (model == null)
                throw new BusiException("登录帐号不存在");
            if (!string.Equals($"{model.Password},{model.PasswordSalt}".Md5(), password))
                throw new BusiException("登录密码不正确");
            return model.MapTo<AccountDto>();
        }

        public async Task<int> UpdateAsync(TAccount model)
        {
            return await Connection.UpdateAsync(model, new[] { nameof(TAccount.Nick), nameof(TAccount.Avatar) }, Trans);
        }
    }
}
