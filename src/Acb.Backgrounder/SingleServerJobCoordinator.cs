using System.Threading.Tasks;

namespace Acb.Backgrounder
{
    /// <inheritdoc />
    /// <summary> 单个任务的任务协调者 </summary>
    public class SingleServerJobCoordinator : IJobCoordinator
    {
        public Task GetWork(IJob job)
        {
            return job.Execute();
        }

        public void Dispose()
        {
        }
    }
}
