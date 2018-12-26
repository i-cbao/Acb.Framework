using System.Threading.Tasks;
using Acb.Core.Domain;

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
            return await Repository.UnitOfWork.Trans(async () =>
            {
                var count = await Repository.UpdateName(id, name);
                count += await Repository.UpdateEmail(id, email);
                return count;
            });
        }
    }
}
