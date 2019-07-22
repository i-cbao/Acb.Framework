using System.Threading.Tasks;
using Acb.Core.Dependency;

namespace Acb.WebApi.Test.Domain.Repositories
{
    public interface IAccountContract : IScopedDependency
    {
        Task<TAccount> QueryById(string id);

        Task<int> Update(string id, string name, string email);
    }
}
