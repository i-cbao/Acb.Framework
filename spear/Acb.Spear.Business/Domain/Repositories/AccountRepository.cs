using System.Threading.Tasks;
using Acb.AutoMapper;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Contracts.Dtos;

namespace Acb.Spear.Business.Domain.Repositories
{
    public class AccountRepository : DapperRepository<TAccount>
    {
        /// <summary> 查询账号 </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<TAccount> QueryAccountAsync(string account)
        {
            return Connection.QueryByIdAsync<TAccount>(account, nameof(TAccount.Account));
        }

        //public Task<TAccount> RegistAsync(TAccount model)
        //{

        //}

        public async Task<AccountDto> LoginAsync(string account, string password)
        {
            var model = await QueryAccountAsync(account);
            if (model == null)
                throw new BusiException("登录帐号不存在");
            if (!string.Equals($"{model.Password},{model.PasswordSalt}".Md5(), password))
                throw new BusiException("登录密码不正确");
            return model.MapTo<AccountDto>();
        }
    }
}
