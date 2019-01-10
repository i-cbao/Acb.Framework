using Acb.Core;
using Acb.Core.Dependency;
using Acb.Spear.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Contracts
{
    /// <inheritdoc />
    /// <summary> 配置相关契约 </summary>
    public interface IConfigContract : IDependency
    {
        /// <summary> 查询项目所有配置名 </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetNamesAsync(Guid projectId);

        /// <summary> 查询已配置的环境 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetEnvsAsync(Guid projectId, string module);

        /// <summary> 获取配置 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        Task<string> GetAsync(Guid projectId, string module, string env = null);

        /// <summary> 获取配置 </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<ConfigDto> DetailAsync(Guid configId);

        /// <summary> 获取版本 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        Task<string> GetVersionAsync(Guid projectId, string module, string env = null);

        /// <summary> 获取历史记录 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<ConfigDto>> GetHistoryAsync(Guid projectId, string module, string env = null, int page = 1,
            int size = 10);

        /// <summary> 还原历史记录 </summary>
        /// <param name="historyId"></param>
        /// <returns></returns>
        Task<ConfigDto> RecoveryAsync(Guid historyId);

        /// <summary> 保存配置 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> SaveAsync(ConfigDto dto);

        /// <summary> 删除配置 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(Guid projectId, string module, string env);

        /// <summary> 删除配置 </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(Guid configId);
    }
}
