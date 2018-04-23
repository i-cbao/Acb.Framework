using System;
using System.Threading.Tasks;

namespace Acb.Backgrounder
{
    /// <summary> 任务协调者 </summary>
    public interface IJobCoordinator : IDisposable
    {
        /// <summary> 获取工作任务 </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Task GetWork(IJob job);
    }
}
