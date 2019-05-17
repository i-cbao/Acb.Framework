using Acb.Core.Dependency;
using System;
using System.Threading.Tasks;

namespace Acb.Core.Logging
{
    /// <summary> 远程日志服务 </summary>
    public interface IRemoteLogger : IScopedDependency, IDisposable
    {
        /// <summary> 记录日志 </summary>
        /// <param name="msg">消息</param>
        /// <param name="level"></param>
        /// <param name="ex">异常信息</param>
        /// <param name="date"></param>
        /// <param name="logger"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        Task Logger(object msg, LogLevel level, Exception ex = null, DateTime? date = null, string logger = null,
            string site = null);
    }
}
