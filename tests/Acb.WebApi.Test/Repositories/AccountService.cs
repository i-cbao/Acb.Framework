using System.Threading.Tasks;

namespace Acb.WebApi.Test.Repositories
{
    public class AccountService : IAccountContract
    {
        public AccountRepository Repository { private get; set; }

        public AccountService()
        {
        }

        public async Task<TAccount> QueryById(string id)
        {
            return await Repository.QueryById(id);
        }

        public async Task<int> Update(string id, string name, string email)
        {
            await Repository.UnitOfWork.BeginTransaction(async () =>
            {
                var count = await Repository.UpdateName(id, name);
                count += await Repository.UpdateEmail(id, email);
                //return count;
            });
            return await Task.FromResult(1);
        }
    }
}
