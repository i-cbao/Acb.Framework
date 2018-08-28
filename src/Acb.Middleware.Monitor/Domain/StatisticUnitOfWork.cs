using Acb.Dapper;

namespace Acb.Middleware.Monitor.Domain
{
    public class StatisticUnitOfWork : UnitOfWork
    {
        public StatisticUnitOfWork() : base("statistic") { }
    }
}
