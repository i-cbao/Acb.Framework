using Acb.Core.Data;

namespace Acb.Middleware.Monitor.Domain
{
    public class StatisticUnitOfWork : UnitOfWork
    {
        public StatisticUnitOfWork() : base("statistic") { }
    }
}
