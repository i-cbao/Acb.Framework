using System;
using System.Threading.Tasks;

namespace Acb.Backgrounder
{
    /// <summary> 基础任务接口 </summary>
    public interface IJob
    {
        /// <summary> 任务名称 </summary>
        string Name { get; }

        /// <summary> 执行任务 </summary>
        /// <returns></returns>
        Task Execute();

        /// <summary> 执行间隔 </summary>
        TimeSpan Interval { get; }

        /// <summary> 超时时间 </summary>
        TimeSpan? TimeOut { get; }

        /// <summary> 开始时间 </summary>
        DateTime? StartTime { get; }

        /// <summary> 失效时间 </summary>
        DateTime? ExpireTime { get; }
    }
}
