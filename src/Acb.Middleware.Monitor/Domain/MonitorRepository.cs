using Acb.Dapper.Domain;
using Acb.Middleware.Monitor.Domain.Entities;

namespace Acb.Middleware.Monitor.Domain
{
    public class MonitorRepository : DapperRepository<TMonitor>
    {
        public MonitorRepository(StatisticUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
