using System.Threading;
using System.Threading.Tasks;
using Acb.Core.Domain.Entities;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.WebApi.Test.Domain.Connections;

namespace Acb.WebApi.Test.Domain.Repositories
{
    [Naming(NamingType.UrlCase, Name = "t_account")]
    public class TAccount : BaseEntity<string>
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class AccountRepository : DapperRepository<TAccount>
    {
        private readonly ILogger _logger;

        public AccountRepository(MainDbContext unitOfWork) : base(unitOfWork)
        {
            _logger = LogManager.Logger<AccountRepository>();
        }

        public async Task<TAccount> QueryById(string id)
        {
            _logger.Debug($"QueryById Thread: {Thread.CurrentThread.ManagedThreadId}");
            return await Connection.QueryByIdAsync<TAccount>(id);
        }

        public async Task<int> UpdateName(string id, string name)
        {
            _logger.Debug($"UpdateName Thread: {Thread.CurrentThread.ManagedThreadId}");
            return await Connection.UpdateAsync(new TAccount { Id = id, Name = name }, new[] { nameof(TAccount.Name) },
                Trans);
        }

        public async Task<int> UpdateEmail(string id, string email)
        {
            _logger.Debug($"UpdateEmail Thread: {Thread.CurrentThread.ManagedThreadId}");
            return await Connection.UpdateAsync(new TAccount { Id = id, Email = email }, new[] { nameof(TAccount.Email) },
                Trans);
        }
    }
}
